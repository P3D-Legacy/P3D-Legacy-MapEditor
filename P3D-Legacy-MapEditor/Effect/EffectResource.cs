using System.IO;

namespace P3D.Legacy.MapEditor.Effect
{
    /// <summary>
    /// Internal helper for accessing the bytecode for stock effects.
    /// </summary>
    internal partial class EffectResource
    {
        public static readonly EffectResource FxaaHighResource 
            = new EffectResource("P3D.Legacy.MapEditor.Effect.Resources.FxaaEffectHigh.dx11.mgfx");
        public static readonly EffectResource FxaaDefaultResource
            = new EffectResource("P3D.Legacy.MapEditor.Effect.Resources.FxaaEffectDefault.dx11.mgfx");
        public static readonly EffectResource FxaaLowResource
            = new EffectResource("P3D.Legacy.MapEditor.Effect.Resources.FxaaEffectLow.dx11.mgfx");

        private readonly object _locker = new object();
        private readonly string _name;
        private volatile byte[] _bytecode;

        private EffectResource(string name)
        {
            _name = name;
        }

        public byte[] Bytecode
        {
            get
            {
                if (_bytecode == null)
                {
                    lock (_locker)
                    {
                        if (_bytecode != null)
                            return _bytecode;

                        var assembly = typeof(EffectResource).Assembly;
                        var stream = assembly.GetManifestResourceStream(_name);
                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            _bytecode = ms.ToArray();
                        }
                    }
                }

                return _bytecode;
            }
        }
    }
}