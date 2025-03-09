using Forge.UX.UI.Elements;
using Forge.UX.UI.Elements.Grouping;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Forge.UX.UI {
    public class SceneTree : ObservableCollection<UIElement> {
        SceneManager? attachedManager;
        UIGroup? owner;

        public void Attach(SceneManager manager, UIGroup? owner) {
            if (attachedManager != null) {
                CollectionChanged += (sender, e) => {
                    RefreshAttachment();
                };
            }

            this.attachedManager = manager;
            this.owner = owner;
            RefreshAttachment();
        }

        private void RefreshAttachment() {
            foreach (UIElement element in this) {
                element.Attach(attachedManager!, owner!);
            }
        }

        public UIElement GetById(string id) {
            return GetAllElementsInTree().FirstOrDefault(e => e.Id == id) ?? throw new KeyNotFoundException($"Element with id {id} not found");
        }
        public UIElement? GetByIdOrNull(string id) {
            return GetAllElementsInTree().FirstOrDefault(e => e.Id == id);
        }

        public T? GetById<T>(string id) where T : UIElement {
            UIElement? foundElement = GetById(id);
            if (foundElement is T t) return t;
            throw new KeyNotFoundException($"Element with id {id} and type {typeof(T).Name} not found");
        }

        public T? GetByIdOrNull<T>(string id) where T : UIElement {
            UIElement? foundElement = GetById(id);
            if (foundElement is T t) return t;
            return null;
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
