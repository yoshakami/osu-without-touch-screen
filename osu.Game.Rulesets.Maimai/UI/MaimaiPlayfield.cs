﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Maimai.Configuration;
using osu.Game.Rulesets.Maimai.Objects.Drawables;
using osu.Game.Rulesets.Maimai.UI.Components;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Menu;
using osuTK;
using osuTK.Graphics;
using System;

namespace osu.Game.Rulesets.Maimai.UI
{
    [Cached]
    public class MaimaiPlayfield : Playfield
    {
        private JudgementContainer<DrawableMaimaiJudgement> judgementLayer;

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        public static readonly float ringSize = 600;
        public static readonly float dotSize = 20f;
        public static readonly float intersectDistance = 296.5f;
        public static readonly float noteStartDistance = 66f;
        public static readonly float[] pathAngles =
            {
                22.5f,
                67.5f,
                112.5f,
                157.5f,
                202.5f,
                247.5f,
                292.5f,
                337.5f
            };

        public MaimaiPlayfield()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.None;
            Size = new Vector2(600);
            AddRangeInternal(new Drawable[]
            {
                judgementLayer = new JudgementContainer<DrawableMaimaiJudgement>
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = 1,
                },
                new VisualisationContainer(),
                HitObjectContainer,
                new MaimaiRing(),
            });
        }
        protected override GameplayCursorContainer CreateCursor() => new MaimaiCursorContainer();


        public override void Add(DrawableHitObject h)
        {
            base.Add(h);

            var obj = (DrawableMaimaiHitObject)h;

            obj.OnNewResult += onNewResult;
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!judgedObject.DisplayResult || !DisplayJudgements.Value)
                return;

            var maimaiObj = (DrawableMaimaiHitObject)judgedObject;

            var b = maimaiObj.HitObject.Angle + 90;
            var a = b *= (float)(Math.PI / 180);

            DrawableMaimaiJudgement explosion = new DrawableMaimaiJudgement(result, maimaiObj)
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Position = new Vector2(-(240 * (float)Math.Cos(a)), -(240 * (float)Math.Sin(a))),
                Rotation = maimaiObj.HitObject.Angle,
            };

            judgementLayer.Add(explosion);
        }


        public class DotPiece : Container
        {
            public DotPiece()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                RelativeSizeAxes = Axes.None;
                Children = new Drawable[] {
                    new Circle
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new CircularContainer{
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        BorderColour = Color4.Black,
                        BorderThickness = 3,
                        Child = new Box
                        {
                            AlwaysPresent = true,
                            Alpha = 0,
                            RelativeSizeAxes = Axes.Both,
                        }
                    },
                };
            }
        }

        private class VisualisationContainer : BeatSyncedContainer
        {
            private readonly float ringSize = 600;

            private LogoVisualisation visualisation;
            private readonly Bindable<bool> showVisualisation = new Bindable<bool>(true);

            [BackgroundDependencyLoader(true)]
            private void load(MaimaiRulesetConfigManager settings)
            {
                FillAspectRatio = 1;
                FillMode = FillMode.Fit;
                // RelativeSizeAxes = Axes.Both;
                Size = new Vector2(ringSize);
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;


                Child = visualisation = new LogoVisualisation
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Color4.Pink.Darken(.8f),

                };

                settings?.BindWith(MaimaiRulesetSettings.ShowVisualizer, showVisualisation);
                showVisualisation.TriggerChange();
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                visualisation.AccentColour = Color4.Pink.Darken(.8f);
            }

            protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, TrackAmplitudes amplitudes)
            {
                if (effectPoint.KiaiMode && showVisualisation.Value)
                {
                    visualisation.FadeIn(200);

                }
                else
                {
                    visualisation.FadeOut(500);
                }
            }
        }
    }
}