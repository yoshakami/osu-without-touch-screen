﻿using osu.Framework.Bindables;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Sentakki.Judgements;
using osu.Game.Rulesets.Sentakki.Scoring;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Objects;
using osuTK;
using osuTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using System.Threading;

namespace osu.Game.Rulesets.Sentakki.Objects
{
    public abstract class SentakkiHitObject : HitObject
    {
        public virtual bool IsBreak { get; set; } = false;
        public virtual bool HasTwin { get; set; } = false;

        public override Judgement CreateJudgement() => new SentakkiJudgement();

        public virtual Color4 NoteColor => IsBreak ? Color4.OrangeRed : (HasTwin ? Color4.Gold : Color4Extensions.FromHex("ff0064"));

        protected override HitWindows CreateHitWindows() => new SentakkiHitWindows();

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            if (IsBreak)
                for (int i = 0; i < 4; ++i)
                    AddNested(new ScorePaddingObject());
        }
    }
}
