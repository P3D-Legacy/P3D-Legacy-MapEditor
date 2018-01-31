using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Effect
{
    public enum FxaaQuality
    {
        High,
        Default,
        Low
    }

    public class FxaaEffect : Microsoft.Xna.Framework.Graphics.Effect
    {
        private EffectParameter _subpixelQuality;
        private EffectParameter _edgeThreshold;
        private EffectParameter _edgeThresholdMin;

        private EffectParameter _inverseDimensions;
        private EffectParameter _renderTarget;

        public float SubpixelQuality
        {
            get => _subpixelQuality.GetValueSingle();
            set => _subpixelQuality.SetValue(value);
        }
        public float EdgeThreshold
        {
            get => _edgeThreshold.GetValueSingle();
            set => _edgeThreshold.SetValue(value);
        }
        public float EdgeThresholdMin
        {
            get => _edgeThresholdMin.GetValueSingle();
            set => _edgeThresholdMin.SetValue(value);
        }

        public Vector2 InverseDimensions
        {
            get => _inverseDimensions.GetValueVector2();
            set => _inverseDimensions.SetValue(value);
        }

        public Texture2D RenderTarget
        {
            get => _renderTarget.GetValueTexture2D();
            set => _renderTarget.SetValue(value);
        }


        public FxaaEffect(GraphicsDevice graphicsDevice, FxaaQuality fxaaQuality = FxaaQuality.High) 
            : base(graphicsDevice, GetFxaaQuality(fxaaQuality))
        {
            CacheEffectParameters(null);
        }
        private static byte[] GetFxaaQuality(FxaaQuality fxaaQuality)
        {
            switch (fxaaQuality)
            {
                case FxaaQuality.High:
                    return EffectResource.FxaaHighResource.Bytecode;

                case FxaaQuality.Default:
                    return EffectResource.FxaaDefaultResource.Bytecode;

                case FxaaQuality.Low:
                    return EffectResource.FxaaLowResource.Bytecode;

                default:
                    throw new ArgumentOutOfRangeException(nameof(fxaaQuality), fxaaQuality, null);
            }
        }
        protected FxaaEffect(FxaaEffect cloneSource) : base(cloneSource)
        {
            CacheEffectParameters(cloneSource);

            _subpixelQuality = cloneSource._subpixelQuality;
            _edgeThreshold = cloneSource._edgeThreshold;
            _edgeThresholdMin = cloneSource._edgeThresholdMin;

            _inverseDimensions = cloneSource._inverseDimensions;
        }


        public void SetHightQuality()
        {
            SubpixelQuality = 1.00f;
            EdgeThreshold = 0.125f;
            EdgeThresholdMin = 0.0625f;
        }

        public void SetDefaultQuality()
        {
            SubpixelQuality = 0.75f;
            EdgeThreshold = 0.166f;
            EdgeThresholdMin = 0.0833f;
        }

        public void SetLowQuality()
        {
            SubpixelQuality = 0.50f;
            EdgeThreshold = 0.250f;
            EdgeThresholdMin = 0.0833f;
        }

        /// <summary>
        /// Looks up shortcut references to our effect parameters.
        /// </summary>
        private void CacheEffectParameters(FxaaEffect cloneSource)
        {
            _subpixelQuality = Parameters["SubpixelQuality"];
            _edgeThreshold = Parameters["EdgeThreshold"];
            _edgeThresholdMin = Parameters["EdgeThresholdMin"];

            _inverseDimensions = Parameters["InverseDimensions"];
            _renderTarget = Parameters["Texture"];
        }
    }
}