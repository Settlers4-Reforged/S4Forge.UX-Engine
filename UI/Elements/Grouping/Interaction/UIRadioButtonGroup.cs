using Forge.UX.UI.Elements.Interaction;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;

namespace Forge.UX.UI.Elements.Grouping.Interaction {
    public class UIRadioButtonGroup<T> : UIGroup, IUIBindable<T>, IUIValueObserver<T> {
        public UIRadioButtonGroup(string groupLinkId) {
            GroupLinkId = groupLinkId;
            Elements ??= new SceneTree();

            Elements.CollectionChanged += SceneTreeChanged;
        }

        private void SceneTreeChanged(object sender, NotifyCollectionChangedEventArgs e) {
            // Update all radio buttons with the same link id
            foreach (UIElement element in Elements.GetAllElementsInTree()) {
                if (element is UIRadioButton<T> rbX) {
                    rbX.OnValueChange += (target, check) => {
                        UIRadioButton<T>? rb = (UIRadioButton<T>)target;

                        // Ignore unchecks
                        if (!check) return;

                        foreach (UIElement e in Elements.GetAllElementsInTree()) {
                            // Uncheck all other radio buttons with the same link id
                            if (e is UIRadioButton<T> rb2 && rb2.LinkId == rb.LinkId && rb2 != rb) {
                                rb2.IsChecked = false;
                            }
                        }
                    };
                }
            }
        }

        public override bool IsTransparent => true;

        public string GroupLinkId { get; set; }


        public UIRadioButton<T>? SelectedRadioButton => Elements.GetAllElementsInTree<UIRadioButton<T>>().FirstOrDefault(element => element is { IsChecked: true });

        public T? SelectedValue => SelectedRadioButton != null ? SelectedRadioButton.Value : default;

        public override void Process(SceneGraphState state) {
            base.Process(state);
        }

        public event BindableGetter<T>? BindingGetValue;
        public event ValueObserverSetter<T>? OnValueChange;

    }
}
