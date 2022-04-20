using PaginationOffsetAspnet.Models;
using System.Collections.Generic;

namespace PaginationOffsetAspnet.ViewModels
{
    public class UserViewModel
    {
        public int Page { get; set; }
        public int ValPerPage { get; set; }
        public IEnumerable<User> Users { get; set; }
        public string SortedValue { get; set; }

    }
}
