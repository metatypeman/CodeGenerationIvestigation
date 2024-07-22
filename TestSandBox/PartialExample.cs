namespace TestSandBox
{
    public partial class PartialExample
    {
        public string StrProp { get => _strField; set => _strField = value; }
        public string StrProp2 
        {
            get { return _strField; }
            set { _strField = value; }
        }

        private string _strField;
    }
}
