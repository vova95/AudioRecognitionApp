using AudioRecognitionApp.BLL.DTO;
using AudioRecognitionApp.BLL.Services;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace AudioRecognitionApp.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            IEnumerable<DesiredSongDTO> matches = new List<DesiredSongDTO>();
            if (file != null && file.ContentLength > 0)
            {

                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
                SearchService searchService = new SearchService();
                matches = searchService.SearchSong(path);
            }

            return View(matches);
        }
    }
}