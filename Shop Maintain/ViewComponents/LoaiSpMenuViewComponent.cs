using Shop_Maintain.Models;
using Microsoft.AspNetCore.Mvc;
using Shop_Maintain.Repository;
namespace Shop_Maintain.ViewComponents
{
    public class LoaiSpMenuViewComponent : ViewComponent
    {
        private readonly IloaiSpRepository _loaiSp;
        public LoaiSpMenuViewComponent(IloaiSpRepository loaiSpRepository)
        {
            _loaiSp = loaiSpRepository;
        }
        public IViewComponentResult Invoke()
        {
            var loaisp = _loaiSp.GetAllLoaiSp().OrderBy(x => x.Loai );
            return View( loaisp );
        }
    }

}
