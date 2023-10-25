using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Forge.UX.UI {
    public class SceneTree : ObservableCollection<UIElement> {
        public UIElement? GetById(string id) {
            return GetAllElementsInTree().FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<UIElement> GetAllElementsInTree() {
            static IEnumerable<UIElement> GetChildElements(SceneTree parent) {
                foreach (UIElement child in parent) {
                    if (child is UIGroup g) {
                        foreach (UIElement childElement in GetChildElements(g.Elements)) yield return childElement;
                    }

                    yield return child;
                }
            }


            foreach (UIElement childElement in GetChildElements(this)) yield return childElement;
        }

        public IEnumerable<T> GetAllElementsInTree<T>() where T : UIElement {
            return GetAllElementsInTree().OfType<T>();
        }

        public void AddRange(IEnumerable<UIElement> elements) {
            foreach (UIElement element in elements) {
                Add(element);
            }
        }
    }
}
