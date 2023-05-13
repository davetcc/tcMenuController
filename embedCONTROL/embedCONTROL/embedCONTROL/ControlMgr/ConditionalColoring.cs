using System;
using embedCONTROL.Services;
using tcMenuControlApi.Serialisation;

namespace embedCONTROL.ControlMgr
{
    public enum ColorComponentType { TEXT_FIELD, BUTTON, HIGHLIGHT, CUSTOM }

    public interface IConditionalColoring
    {
        PortableColor ForegroundFor(RenderStatus status, ColorComponentType compType);
        PortableColor BackgroundFor(RenderStatus status, ColorComponentType compType);
    }

    public class NullConditionalColoring : IConditionalColoring
    {
        public PortableColor BackgroundFor(RenderStatus status, ColorComponentType ty)
        {
            return PortableColors.WHITE;
        }

        public PortableColor ForegroundFor(RenderStatus status, ColorComponentType ty)
        {
            return PortableColors.BLACK;
        }
    }

    public class PrefsConditionalColoring : IConditionalColoring
    {
        private readonly PrefsAppSettings _settings;

        public PrefsConditionalColoring(PrefsAppSettings settings)
        {
            _settings = settings;
        }

        public PortableColor BackgroundFor(RenderStatus status, ColorComponentType compType)
        {
            switch (status)
            {
                case RenderStatus.RecentlyUpdated:
                    return _settings.UpdateColor.Bg;
                case RenderStatus.EditInProgress:
                    return _settings.PendingColor.Bg;
                case RenderStatus.CorrelationError:
                    return _settings.ErrorColor.Bg;
                default:
                    return ColorCompForType(compType).Bg;
            }
        }

        private ControlColor ColorCompForType(ColorComponentType compType)
        {
            switch (compType)
            {
                case ColorComponentType.TEXT_FIELD:
                    return _settings.TextColor;
                case ColorComponentType.BUTTON:
                    return _settings.ButtonColor;
                case ColorComponentType.HIGHLIGHT:
                    return _settings.HighlightColor;
                case ColorComponentType.CUSTOM:
                    return _settings.HighlightColor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compType), compType, null);
            }
        }

        public PortableColor ForegroundFor(RenderStatus status, ColorComponentType compType)
        {
            switch (status)
            {
                case RenderStatus.RecentlyUpdated:
                    return _settings.UpdateColor.Fg;
                case RenderStatus.EditInProgress:
                    return _settings.PendingColor.Fg;
                case RenderStatus.CorrelationError:
                    return _settings.ErrorColor.Fg;
                default:
                    return ColorCompForType(compType).Fg;
            }
        }
    }
}
