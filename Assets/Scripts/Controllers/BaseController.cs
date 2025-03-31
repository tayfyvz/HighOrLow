namespace Controllers
{
    public abstract class BaseController<TView>
    {
        protected TView View { get; private set; }

        public virtual void AttachView(TView view)
        {
            View = view;
            UpdateView();
        }
        
        protected abstract void UpdateView();
    }
}