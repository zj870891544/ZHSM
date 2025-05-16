namespace ZHSM
{
    public class ServerMenuForm : UGuiForm
    {
        private ProcedureMenu procedureMenu;
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            procedureMenu = userData as ProcedureMenu;
        }

        public void EnterRoom()
        {
            procedureMenu.StartGame();
        }
    }
}