using Godot;
using System;

public partial class Player_mov : CharacterBody2D
{
	// Platformer Toolkit values (converted)
	private const float PIXELS_PER_UNIT = 1f;

	// Movement
	public const float MAX_SPEED = 6f * 60f;
	public const float ACCEL = 16f * 60f;
	public const float DECEL = 60f * 60f;
	public const float TURN_SPEED = 70f * 60f;
	public const bool INSTANT_MOVEMENT = false;

	// Jumping
	public const float JUMP_HEIGHT = 3.5f * 16f;
	public const float JUMP_DURATION = 3.8f / 60f;
	public const float GRAVITY_DOWN = 3f * 60f * 60f;
	public const float JUMP_VELOCITY = -(2f * JUMP_HEIGHT) / JUMP_DURATION;
	public const float JUMP_CUTOFF_MULTIPLIER = 0.6f;

	// Air control
	public const float AIR_ACCEL = 20f * 60f;
	public const float AIR_CONTROL = 40f;
	public const float AIR_BRAKE = 20f * 60f;

	// Assists
	public const float COYOTE_TIME_MAX = 0.1f;
	public const float JUMP_BUFFER_MAX = 0.2f;

	// State
	private float coyoteTimeCounter = 0f;
	private float jumpBufferCounter = 0f;
	private bool jumpHeld = false;

	// Gameplay
	public int n_coins = 0;

	// Footstep audio
	private AudioStreamPlayer2D footstepAudio;

	public override void _Ready()
	{
		footstepAudio = GetNode<AudioStreamPlayer2D>("Footstepaudio");
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		Vector2 velocity = Velocity;

		// COYOTE TIME
		if (IsOnFloor())
			coyoteTimeCounter = COYOTE_TIME_MAX;
		else
			coyoteTimeCounter -= dt;

		// JUMP BUFFER
		if (Input.IsActionJustPressed("ui_accept"))
			jumpBufferCounter = JUMP_BUFFER_MAX;
		else
			jumpBufferCounter -= dt;

		// VARIABLE HEIGHT (cutoff)
		if (!Input.IsActionPressed("ui_accept") && jumpHeld && velocity.Y < 0)
			velocity.Y *= JUMP_CUTOFF_MULTIPLIER;

		// JUMP EXECUTION
		if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
		{
			velocity.Y = JUMP_VELOCITY;
			coyoteTimeCounter = 0f;
			jumpBufferCounter = 0f;
			jumpHeld = true;
		}

		// GRAVITY
		if (!IsOnFloor())
			velocity.Y += GRAVITY_DOWN * dt;
		else
			jumpHeld = false;

		// MOVEMENT
		float input = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		float accel = IsOnFloor() ? ACCEL : AIR_ACCEL;
		float targetSpeed = input * MAX_SPEED;

		if (INSTANT_MOVEMENT)
		{
			velocity.X = targetSpeed;
		}
		else
		{
			if (input != 0)
			{
				float speedDiff = targetSpeed - velocity.X;
				float direction = Mathf.Sign(speedDiff);
				float appliedAccel = direction * (Mathf.Sign(velocity.X) != direction ? TURN_SPEED : accel) * dt;
				velocity.X = Mathf.MoveToward(velocity.X, targetSpeed, Mathf.Abs(appliedAccel));
			}
			else
			{
				float decel = IsOnFloor() ? DECEL : AIR_BRAKE;
				velocity.X = Mathf.MoveToward(velocity.X, 0, decel * dt);
			}
		}

		// --- FOOTSTEP SOUND ---
		bool isMoving = Mathf.Abs(velocity.X) > 5f;
		bool onGround = IsOnFloor();

		if (isMoving && onGround)
		{
			if (!footstepAudio.Playing)
				footstepAudio.Play();
		}
		else
		{
			footstepAudio.Stop();
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
