using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public abstract class UIController : MonoBehaviour
    {
        private readonly List<UIView> registeredViews = new();

        public T GetView<T>() where T : UIView
        {
            foreach (var view in registeredViews)
            {
                if (view is T result)
                    return result;
            }
            return null;
        }

        public void RegisterView(UIView view)
        {
            if (!registeredViews.Contains(view))
            {
                registeredViews.Add(view);
            }
        }

        public void UnregisterView(UIView view)
        {
            registeredViews.Remove(view);
        }

        protected virtual void Awake()
        {
            InitializeViews();
        }

        protected virtual void Start()
        {
            OnControllerStart();
        }

        protected virtual void OnControllerStart() { }

        protected virtual void InitializeViews()
        {
            var views = GetComponentsInChildren<UIView>(true);
            foreach (var view in views)
            {
                view.Initialize(this);
            }
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void ShowAll()
        {
            Show();
            foreach (var view in registeredViews)
            {
                view.Show();
            }
        }

        public virtual void HideAll()
        {
            foreach (var view in registeredViews)
            {
                view.Hide();
            }
            Hide();
        }
    }
}