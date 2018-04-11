
namespace dbRad
{
    
   public partial class Settings
    {
        private string _Setting1 = "Setting1";
        private string _Setting2 = "Setting2";
        private string _Setting3 = "Setting3";
        private string _Setting4 = "Setting4";

        public string Setting1
        {
            get
            {
                return _Setting1;
            }
            set
            {
                _Setting1 = value;
            }
        }

        public string Setting2
        {
            get
            {
                return _Setting2;
            }
            set
            {
                _Setting2 = value;
            }
        }

        public string Setting3
        {
            get
            {
                return _Setting3;
            }
            set
            {
                _Setting3 = value;
            }
        }

        public string Setting4
        {
            get
            {
                return _Setting4;
            }
            set
            {
                _Setting4 = value;
            }
        }

    }
}
