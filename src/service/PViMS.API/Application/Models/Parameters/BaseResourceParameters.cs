namespace PVIMS.API.Models.Parameters
{
    public abstract class BaseResourceParameters
    {
        const int _maxPageSize = 50;

        /// <summary>
        /// The page number that you would like returned. Review the X-Pagination header in the response to view total number  
        /// of pages
        /// </summary>
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        /// <summary>
        /// The page size that you would like returned. Review the X-Pagination header in the response to view total number  
        /// of pages based on the requested page size. A default page size of 10 is used if no page size is requested  
        /// and the maximum page size is 20. Any request above 20 will be ignored and 20 will be used.
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > _maxPageSize) ? 99999 : value;
            }
        }
    }
}
