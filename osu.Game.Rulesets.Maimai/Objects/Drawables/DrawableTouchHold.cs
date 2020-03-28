﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Maimai.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;
using System.Linq;

namespace osu.Game.Rulesets.Maimai.Objects.Drawables
{
    public class DrawableTouchHold : DrawableMaimaiHitObject
    {
        private readonly TouchHoldCircle circle;

        public override bool HandlePositionalInput => true;

        private MaimaiInputManager maimaiActionInputManager;
        internal MaimaiInputManager MaimaiActionInputManager => maimaiActionInputManager ??= GetContainingInputManager() as MaimaiInputManager;

        protected override double InitialLifetimeOffset => 500;

        public DrawableTouchHold(TouchHold hitObject)
            : base(hitObject)
        {
            AccentColour.Value = Color4.HotPink;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(120);
            Scale = new Vector2(0f);
            RelativeSizeAxes = Axes.None;
            Alpha = 0;
            AddRangeInternal(new Drawable[] {
                circle = new TouchHoldCircle(){ Duration = hitObject.Duration },
            });
        }

        protected override void UpdateInitialTransforms()
        {
            this.FadeTo(.5f, 500).ScaleTo(.8f, 500);
            circle.StartProgressBar();
            if (isHidden)
                using (BeginDelayedSequence(500))
                {
                    this.FadeOut(250);
                }
        }

        private double potential = 0;
        private double held = 0;
        private bool buttonHeld = false;
        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (Time.Current < HitObject.StartTime) return;

            if (userTriggered || Time.Current < (HitObject as IHasEndTime)?.EndTime)
                return;

            double result = held / potential;

            ApplyResult(r =>
            {
                if (result >= .9)
                    r.Type = HitResult.Perfect;
                else if (result >= .8)
                    r.Type = HitResult.Great;
                else if (result >= .5)
                    r.Type = HitResult.Good;
                else if (result >= .2)
                    r.Type = HitResult.Ok;
                else if (Time.Current >= (HitObject as IHasEndTime)?.EndTime)
                    r.Type = HitResult.Miss;
            });
        }

        public bool Auto = false;
        protected override void Update()
        {
            buttonHeld = MaimaiActionInputManager?.PressedActions.Any(x => x == MaimaiAction.Button1 || x == MaimaiAction.Button2) ?? false;
            if (Time.Current >= HitObject.StartTime && Time.Current <= (HitObject as IHasEndTime)?.EndTime)
            {
                potential++;
                if ((buttonHeld && IsHovered) || Auto)
                {
                    held++;
                    this.FadeTo((isHidden) ? .2f : 1f, 100);
                    this.ScaleTo(1f, 100);
                    circle.Glow.FadeTo(1f, 100);
                }
                else
                {
                    this.FadeTo((isHidden) ? 0 : .5f, 100);
                    this.ScaleTo(.9f, 200);
                    circle.Glow.FadeTo(0f, 200);
                }
                base.Update();
            }
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);
            const double time_fade_hit = 400, time_fade_miss = 400;

            switch (state)
            {
                case ArmedState.Hit:
                    this.Delay((HitObject as IHasEndTime).Duration).ScaleTo(1f, time_fade_hit).Expire();
                    break;

                case ArmedState.Miss:
                    this.Delay((HitObject as IHasEndTime).Duration).ScaleTo(.0f, 400).FadeOut(400);
                    break;
            }
        }
    }
}
