namespace MyBGList.DTO
{
    public class RestDTO<T>
    {
        public T Data { get; set; } = default!;

        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
        public int PageIndex { get; internal set; }
        public int PageSize { get; internal set; }
        public int RecordCount { get; internal set; }
    }
}
