namespace Music_Shop.Data
{
    public class JsonDeserializationException : Exception
    {
        public JsonDeserializationException() { }
        public JsonDeserializationException(string message) : base(message) { }
        public JsonDeserializationException(string message, Exception inner) : base(message, inner) { }
    }
}
