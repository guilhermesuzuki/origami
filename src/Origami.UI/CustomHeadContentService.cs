using Microsoft.AspNetCore.Components;

namespace Origami.UI
{
    public class CustomHeadContentService
    {
        private RenderFragment? _currentContent;

        public event Action? OnChange;

        public RenderFragment? CurrentContent => _currentContent;

        public void SetContent(RenderFragment? content)
        {
            _currentContent = content;
            OnChange?.Invoke();
        }
    }

}
