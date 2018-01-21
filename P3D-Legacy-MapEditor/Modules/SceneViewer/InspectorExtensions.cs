using System;
using System.Linq.Expressions;

using Gemini.Modules.Inspector;

using P3D.Legacy.MapEditor.Modules.SceneViewer.Inspectors;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer
{
    public static class InspectorExtensions
    {
        public static TBuilder WithFloatEditor<TBuilder, T>(this InspectorBuilder<TBuilder> builder, T instance,
            Expression<Func<T, float>> propertyExpression) where TBuilder : InspectorBuilder<TBuilder> =>
            builder.WithEditor<T, float, FloatEditorViewModel>(instance, propertyExpression);
    }
}