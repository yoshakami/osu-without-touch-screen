using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Sentakki.Configuration;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Sentakki.UI.Components.HitObjectLine
{
    public class HitObjectLine : PoolableDrawable
    {
        public override bool RemoveCompletedTransforms => false;

        public LineLifetimeEntry Entry;

        private Sprite sprite;
        public HitObjectLine()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Scale = new Vector2(.11f);
            Size = new Vector2(600);
            Alpha = 0;
        }

        private readonly BindableDouble animationDuration = new BindableDouble(1000);

        private TextureStore textures;

        [BackgroundDependencyLoader(true)]
        private void load(SentakkiRulesetConfigManager sentakkiConfigs, TextureStore textureStore)
        {
            textures = textureStore;
            sentakkiConfigs?.BindWith(SentakkiRulesetSettings.AnimationDuration, animationDuration);
            animationDuration.BindValueChanged(_ => resetAnimation());

            AddInternal(sprite = new Sprite()
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            Colour = Entry.Colour;
            Rotation = Entry.Rotation;
            sprite.Texture = textures.Get(Entry.GetLineTexturePath());

            resetAnimation();
        }

        private void resetAnimation()
        {
            ApplyTransformsAt(double.MinValue);
            ClearTransforms();

            this.FadeIn(animationDuration.Value / 2).Then().ScaleTo(1, animationDuration.Value / 2).Then().FadeOut();
        }
    }
}
