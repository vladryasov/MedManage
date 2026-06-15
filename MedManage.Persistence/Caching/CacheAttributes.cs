namespace MedManage.Persistence.Caching
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : Attribute
    {
        public string KeyTemplate { get; }          // например "Announcement:ById:{id}"
        public int ExpirationSeconds { get; set; } = 1800; // по умолчанию 30 минут

        public CacheAttribute(string keyTemplate)
        {
            KeyTemplate = keyTemplate;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CacheInvalidateAttribute : Attribute
    {
        public string[] KeyTemplates { get; } // шаблоны ключей для удаления

        public CacheInvalidateAttribute(params string[] keyTemplates)
        {
            KeyTemplates = keyTemplates;
        }
    }
}
