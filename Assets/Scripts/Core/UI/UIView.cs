using UnityEngine;

namespace Game.Core
{
    public abstract class UIView : MonoBehaviour
    {
        protected UIController controller;

        public virtual void Initialize(UIController controller)
        {
            this.controller = controller;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void UpdateView() { }

        protected virtual void OnDestroy()
        {
            if (controller != null)
            {
                controller.UnregisterView(this);
            }
        }
    }
}