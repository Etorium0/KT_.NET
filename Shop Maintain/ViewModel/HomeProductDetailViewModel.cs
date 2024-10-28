using Shop_Maintain.Models;

namespace Shop_Maintain.ViewModel
{
    public class HomeProductDetailViewModel
    {
        public TDanhMucSp danhMucSp {  get; set; }
        public List<TAnhSp> anhSps { get; set; }
    }
}
