using Artentus.GameUtils.Graphics;
using Artentus.GameUtils.UI;

namespace FillTheRow.UI
{
    public abstract class Menu : ContainerElement
    {
        readonly Menu menuParent;

        protected Menu(Menu parent)
        {
            menuParent = parent;
            Visible = false;
            Size = new Vector2(1, 1);
            CanGetFocus = false;
        }

        protected void GoUp()
        {
            if (menuParent != null)
            {
                Visible = false;
                menuParent.Visible = true;
            }
        }

        protected void GoDown(Menu child)
        {
            Visible = false;
            child.Visible = true;
        }
    }
}
