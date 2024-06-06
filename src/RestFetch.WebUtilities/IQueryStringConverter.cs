namespace RestFetch.WebUtilities;

public interface IQueryStringConverter<T>: IQueryStringSerializer<T>, IQueryStringDeserializer<T>;