using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Sentakki.Judgements;
using osu.Game.Rulesets.Sentakki.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Sentakki.Objects
{
    public class SlideBody : SentakkiLanedHitObject, IHasDuration
    {
        public override Color4 DefaultNoteColour => Color4.Aqua;

        public double EndTime
        {
            get => StartTime + Duration;
            set => Duration = value - StartTime;
        }

        public double Duration
        {
            get => SlideInfo.Duration;
            set => SlideInfo.Duration = value;
        }

        public SlideBodyInfo SlideInfo { get; set; }

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            CreateSlideCheckpoints();
            if (NestedHitObjects.Any())
                NestedHitObjects.First().Samples.Add(new SentakkiHitSampleInfo("slide"));
        }

        protected virtual void CreateSlideCheckpoints()
        {
            double distance = SlideInfo.SlidePath.TotalDistance;
            int nodeCount = (int)Math.Floor(distance / 100);
            for (int i = 0; i < nodeCount; i++)
            {
                double progress = (double)(i + 1) / nodeCount;

                SlideCheckpoint checkpoint = new SlideCheckpoint()
                {
                    Progress = (float)progress,
                    StartTime = StartTime + ShootDelay + ((Duration - ShootDelay) * progress)
                };
                if (progress * distance > SlideInfo.SlidePath.FanStartDistance)
                {
                    checkpoint.NodesToPass = 2;

                    for (int j = -1; j < 2; ++j)
                        checkpoint.NodePositions.Add(SlideInfo.SlidePath.PositionAt(progress, j));
                }
                else
                    checkpoint.NodePositions = new List<Vector2> { SlideInfo.SlidePath.PositionAt(progress) };

                AddNested(checkpoint);
            }
        }

        [JsonIgnore]
        public double ShootDelay { get; private set; }

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            double delay = controlPointInfo.TimingPointAt(StartTime).BeatLength * SlideInfo.ShootDelay / 2;
            if (delay < Duration - 50)
                ShootDelay = delay;
        }

        protected override HitWindows CreateHitWindows() => new SentakkiSlideHitWindows();
        public override Judgement CreateJudgement() => new SentakkiJudgement();

        public class SlideNode : SentakkiHitObject
        {
            public virtual float Progress { get; set; }

            protected override HitWindows CreateHitWindows() => HitWindows.Empty;
            public override Judgement CreateJudgement() => new IgnoreJudgement();
        }
    }
}
