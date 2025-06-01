namespace Application.Utilities.Dto
{
    public record DtoMapOptions
    {
        public bool IgnoreNull { get; set; } = false;
    }

    public static class DtoUtilities
    {
        public static TTarget Map<TTarget, TSource>(
            TTarget target,
            TSource source,
            DtoMapOptions? options = null
        )
        {
            var newObject = target;
            DtoMapOptions mapOptions = options ?? new DtoMapOptions();

            foreach (var sourceProperty in typeof(TSource).GetProperties())
            {
                var newValue = sourceProperty.GetValue(source);
                bool ignoreProperty = mapOptions.IgnoreNull && newValue == null;

                if (!ignoreProperty)
                {
                    var targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);

                    if (targetProperty != null && targetProperty.CanWrite)
                    {
                        targetProperty.SetValue(newObject, newValue);
                    }
                }
            }

            return newObject;
        }
    }
}
