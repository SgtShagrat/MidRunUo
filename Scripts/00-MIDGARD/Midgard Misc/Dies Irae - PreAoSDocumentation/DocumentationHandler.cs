namespace Midgard.Misc
{
    public abstract class DocumentationHandler
    {
        public bool Enabled { get; set; }

        public abstract void GenerateDocumentation();

        protected DocumentationHandler()
        {
        }
    }
}