using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using TwitcherSharp.Extensions;
using TwitcherSharp.Reward;
using Timer = Godot.Timer;

namespace Temptica.Overlay.Scripts;

public partial class OtterSprite : MeshInstance3D
{
	private static readonly StringName ParameterDivisions = "pixel_divisions";
	[Export] private ShaderMaterial _pixelShader;
	[Export] private PackedScene _chunkScene;
	private ShaderMaterial _defaultMaterial;
	private CameraTexture _texture;

	private bool _isPixelated;
	private DateTime? _pixelatedEndTime;
	private Timer _explosionTimer;
	private Timer _awaitingVtuberTimer;
	private static CameraFeed CameraFeed { get; set; }
	private TwitchRedeemListener PixelateRedeemListener { get; set; }
	private TwitchReward _pixelateMoreReward;

	private sealed record ChunkData(RigidBody3D Chunk, Vector3 TargetPosition);

	private List<ChunkData> _activeChunks = [];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pixelateMoreReward = TwitchReward.FromObject(GD.Load<GodotObject>("res://Twitcher/pixelate_more.tres"));
		
		if(!TrySetVtuber()){
			_awaitingVtuberTimer = new Timer();
			AddChild(_awaitingVtuberTimer);
			_awaitingVtuberTimer.Start(5d);
			_awaitingVtuberTimer.Timeout += () =>
			{
				if (!TrySetVtuber()) return;
				_awaitingVtuberTimer.Stop();
				RemoveChild(_awaitingVtuberTimer);
				_awaitingVtuberTimer = null;
			};
		}

		PixelateRedeemListener = this.GetTwitcherNode<TwitchRedeemListener>("./PixelateListener");
		PixelateRedeemListener.Redeemed += _ => 
		{
			if (_pixelatedEndTime == null)
			{
				CreatePixelation();
				return;
			}

			HalfPixels();

			_pixelatedEndTime = _pixelatedEndTime.Value.AddSeconds(10);
		};
	}

	private bool TrySetVtuber()
	{
		CameraFeed = CameraServer.Feeds().FirstOrDefault(f => f.GetName().Contains("OBS"));
		_defaultMaterial = (ShaderMaterial)MaterialOverride;
		_texture = _defaultMaterial.GetShaderParameter("texture_albedo").As<CameraTexture>();
		if (CameraFeed == null) return false;

		CameraFeed.SetFormat(0, new Dictionary());
		_texture.SetCameraFeedId(CameraFeed.GetId());
		_texture.WhichFeed = CameraServer.FeedImage.RgbaImage;
		_defaultMaterial.SetShaderParameter("texture_albedo", _texture);
		CameraFeed.FeedIsActive = true;
		Visible = true;

		return true;
	}

	private void CreatePixelation()
	{
		_pixelatedEndTime = DateTime.Now.AddSeconds(60);
		MaterialOverride = _pixelShader;
		_pixelShader.SetShaderParameter(ParameterDivisions, 64);
		PixelateRedeemListener.AddReward(_pixelateMoreReward);
		PixelateRedeemListener.EnsureSubscription();
	}

	private void HalfPixels()
	{
		var pixels = _pixelShader.GetShaderParameter(ParameterDivisions).AsInt32();
		if (pixels == 4)
		{
			//Overlay.SignalRService.PixelateEnd();
			_pixelShader.SetShaderParameter(ParameterDivisions, 24);
			ExplodeAvatar().RunSynchronously();
			return;
		}

		_pixelShader.SetShaderParameter(ParameterDivisions, pixels / 2);
	}

	private async Task ExplodeAvatar()
	{
		// Ensure we are on the main thread for Godot operations
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		var originalTexture = _defaultMaterial.GetShaderParameter("texture_albedo").As<CameraTexture>();
		var textureImage = originalTexture.GetImage();
		Visible = false;

		if (textureImage == null)
		{
			GD.PrintErr("OtterSprite: Could not get Image from texture for color sampling.");
			return; // Early exit if no texture image
		}

		_activeChunks = [];

		// Calculate effective texture size for pixel sampling
		var textureSize = textureImage.GetSize();

		// Get the current pixel_divisions value to scale the UV sampling correctly
		var currentPixelDivisions = _pixelShader.GetShaderParameter(ParameterDivisions).AsInt32();
		var halfDivisions = currentPixelDivisions / 2.0f;

		var unitOffsetX = Scale.X / currentPixelDivisions;
		var unitOffsetY = Scale.Y / currentPixelDivisions;

		// --- CHUNK CREATION LOOP ---
		for (var x = 0; x < currentPixelDivisions; x++)
		{
			for (var y = 0; y < currentPixelDivisions; y++)
			{
				var chunkNode = _chunkScene.Instantiate<RigidBody3D>();
				GetParent().AddChild(chunkNode);

				var localOffsetX = (x - halfDivisions + 0.5f) * unitOffsetX;
				var localOffsetY = (y - halfDivisions + 0.5f) * -unitOffsetY; // Negate Y for usual screen coordinates

				var startPosition = GlobalPosition;
				startPosition.X += localOffsetX;
				startPosition.Y += localOffsetY;

				chunkNode.GlobalPosition = startPosition;


				var chunkScaleX = Scale.X / currentPixelDivisions;
				var chunkScaleY = Scale.Y / currentPixelDivisions;
				var initialChunkScale = new Vector3(chunkScaleX, chunkScaleY, 0.1f);

				chunkNode.Scale = initialChunkScale;

				var chunkData = new ChunkData(chunkNode, startPosition);
				_activeChunks.Add(chunkData);

				var mesh = chunkNode.GetNode<MeshInstance3D>("MeshInstance3D");
				var pixelColor = GetChunkColor(textureImage, x, y, currentPixelDivisions, currentPixelDivisions,
					textureSize);

				if (mesh is { MaterialOverride: StandardMaterial3D chunkMaterial })
				{
					chunkMaterial.AlbedoColor = pixelColor;
				}
				else if (mesh?.Mesh.SurfaceGetMaterial(0) is StandardMaterial3D surfMaterial)
				{
					surfMaterial.AlbedoColor = pixelColor;
				}

				// 3. Apply a randomized force for the "explosion" to the left upwards direction
				var impulseDirection = new Vector3(
					GD.Randf() * -50f,
					GD.Randf() * 50f,
					0
				).Normalized();

				chunkNode.ApplyCentralImpulse(impulseDirection * 50.0f);
			}
		}

		// 4. Explosion Phase: Wait a moment for the pieces to fly out
		await ToSignal(GetTree().CreateTimer(7f), Timer.SignalName.Timeout);

		// 5. Rollback Phase: Visually collect the pieces back to the original spot
		await RollbackChunks(); // Rollback over 0.75 seconds

		// 6. Final Reset and "Build"
		ResetAfterExplosion();
	}

	private static Color GetChunkColor(Image textureImage, int x, int y, int chunkSizeX, int chunkSizeY,
		Vector2I textureSize)
	{
		var uvX = (float)x / chunkSizeX + 0.5f / chunkSizeX;
		var uvY = (float)y / chunkSizeY + 0.5f / chunkSizeY;

		var pixelX = Mathf.Clamp((int)(uvX * textureSize.X), 0, textureSize.X - 1);
		var pixelY = Mathf.Clamp((int)(uvY * textureSize.Y), 0, textureSize.Y - 1);

		return textureImage.GetPixel(pixelX, pixelY);
	}

	// Rollback logic using Tweens and awaiting the first Tween's completion
	private async Task RollbackChunks()
	{
		MaterialOverride = _defaultMaterial;

		const float fadeDuration = 0.5f;
		var chunkTween = CreateTween();

		//modulate to white
		chunkTween.SetParallel();

		foreach (var (chunk, targetPosition) in _activeChunks) // Iterate over the new structure
		{
			// Stop physics and movement
			chunk.PhysicsMaterialOverride = null;
			chunk.LinearVelocity = Vector3.Zero;
			chunk.AngularVelocity = Vector3.Zero;
			chunk.Freeze = true;

			// Create a Tween for smooth movement
			// 1. Tween the position back to its ORIGINAL GRID SPOT
			chunkTween.TweenProperty(chunk, "global_position", targetPosition, fadeDuration) // Use data.TargetPosition
				.SetEase(Tween.EaseType.InOut); // A smooth start and end

			// 2. Tween the rotation back to zero (essential for the rebuild look)
			chunkTween.TweenProperty(chunk, "rotation", Vector3.Zero, fadeDuration)
				.SetEase(Tween.EaseType.InOut);
		}

		await ToSignal(chunkTween, Tween.SignalName.Finished);
		await ToSignal(GetTree().CreateTimer(1), Timer.SignalName.Timeout);

		var tweenFade = CreateTween();
		tweenFade.SetParallel();
		foreach (var (chunk, targetPosition) in _activeChunks)
		{
			tweenFade
				.TweenProperty(chunk, "global_position", targetPosition + new Vector3(10, -10, 0),
					fadeDuration) // Use data.TargetPosition
				.SetEase(Tween.EaseType.InOut);
		}

		await ToSignal(chunkTween, Tween.SignalName.Finished);

		Visible = true;
	}


	private void ResetAfterExplosion()
	{
		foreach (var data in _activeChunks)
		{
			data.Chunk.QueueFree();
		}

		_activeChunks.Clear();
		_pixelatedEndTime = null;
		//Overlay.SignalRService.PixelateEnd();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_pixelatedEndTime == null || !(_pixelatedEndTime < DateTime.Now)) return;
		_pixelatedEndTime = null;
		MaterialOverride = null;

		//Overlay.SignalRService.PixelateEnd();
	}
}
