using AudioRecognitionApp.BLL.DTO;
using AudioRecognitionApp.BLL.Services;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace AudioRecognitionApp.Controllers
{
    [RoutePrefix("admin")]
    public class AdminController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            SongService songService = new SongService();
            List<SongDTO> songs = songService.GetSongsWithPagination(0, 5);
            BaseInformationDTO inf = new BaseInformationDTO
            {
                NumberOfPages = songService.CountPages(),
                Songs = songs
            };
            return View("Index", inf);
        }

        [Route("page/{page}")]
        public ActionResult Paginate(int page)
        {
            SongService songService = new SongService();
            List<SongDTO> songs = songService.GetSongsWithPagination(page - 1, 5);
            BaseInformationDTO inf = new BaseInformationDTO
            {
                NumberOfPages = songService.CountPages(),
                Songs = songs
            };
            return View("Index", inf);
        }
        [Route("create")]
        public ActionResult Create()
        {
            return View("Create");
        }

        [Route("create")]
        [HttpPost]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files)
        {
            SongService songService = new SongService();
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    file.SaveAs(path);
                    songService.CreateSong(path);
                }
            }
            List<SongDTO> songs = songService.GetSongsWithPagination(0, 5);
            BaseInformationDTO inf = new BaseInformationDTO
            {
                NumberOfPages = songService.CountPages(),
                Songs = songs
            };
            return View("Index", inf);
        }

        [Route("{id}/edit")]
        public ActionResult Edit(int id, int page)
        {
            return View();
        }

        [Route("delete")]
        public ActionResult Delete(int id)
        {
            SongService songService = new SongService();
            songService.DeleteSong(id);
            List<SongDTO> songs = songService.GetSongsWithPagination(0, 5);
            BaseInformationDTO inf = new BaseInformationDTO
            {
                NumberOfPages = songService.CountPages(),
                Songs = songs
            };
            return View("Index", inf);
        }

        [Route("Edit/{id}")]
        public ActionResult Edit(int id)
        {
            SongService service = new SongService();
            SongDTO song = service.GetSong(id);
            return View("Edit", song);
        }

        [Route("Edit/{id}")]
        [HttpPost]
        public ActionResult Edit(int id, string name)
        {
            SongService service = new SongService();
            service.UpdateSongName(new SongDTO { Id = id, Name = name });

            List<SongDTO> songs = service.GetSongsWithPagination(0, 5);
            BaseInformationDTO inf = new BaseInformationDTO
            {
                NumberOfPages = service.CountPages(),
                Songs = songs
            };
            return View("Index", inf);
        }
    }
}