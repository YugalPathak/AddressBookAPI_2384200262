using ModelLayer.Model;

namespace ModelLayer.Model
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<AddressBookEntry> Data { get; set; }
    }
}