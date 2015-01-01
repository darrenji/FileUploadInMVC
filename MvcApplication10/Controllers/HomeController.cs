using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ImageResizer;
using MvcApplication10.Models;

namespace MvcApplication10.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        //接收上传图片
        [HttpPost]
        public ActionResult UploadFile()
        {
            //允许的图片格式
            var allowedExtensions = new[] { ".png", ".gif", ".jpg", ".jpeg" };

            //返回给前台的结果，最终以json返回
            List<UploadFileResult> results = new List<UploadFileResult>();

            //遍历从前台传递而来的文件
            foreach (string file in Request.Files)
            {
                //把每个文件转换成HttpPostedFileBase
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;

                //如果前台传来的文件为null，继续遍历其它文件
                if (hpf.ContentLength == 0 || hpf == null)
                {
                    continue;
                }
                else
                {
                    if (hpf.ContentLength > 1024*1024) //如果大于规定最大尺寸
                    {
                        results.Add(new UploadFileResult()
                        {
                            FileName = "",
                            FilePath = "",
                            IsValid = false,
                            Length = hpf.ContentLength,
                            Message = "图片尺寸不能超过1024KB",
                            Type = hpf.ContentType
                        });
                    }
                    else
                    {
                        var extension = Path.GetExtension(hpf.FileName);

                        if (!allowedExtensions.Contains(extension))//如果文件的后缀名不包含在规定的后缀数组中
                        {
                            results.Add(new UploadFileResult()
                            {
                                FileName = "",
                                FilePath = "",
                                IsValid = false,
                                Length = hpf.ContentLength,
                                Message = "图片格式必须是png、gif、jpg或jpeg",
                                Type = hpf.ContentType
                            });
                        }
                        else
                        {
                            //给上传文件改名
                            string date = DateTime.Now.ToString("yyyyMMddhhmmss");
                            //目标文件夹的相对路径 ImageSize需要的格式
                            string pathForSaving = Server.MapPath("~/AjaxUpload/");
                            //目标文件夹的相对路径 统计文件夹大小需要的格式
                            string pathForSaving1 = Server.MapPath("~/AjaxUpload");

                            //在根目录下创建目标文件夹AjaxUpload
                            if (this.CreateFolderIfNeeded(pathForSaving))
                            {
                                //保存小图
                                var versions = new Dictionary<string, string>();
                                versions.Add("_small", "maxwidth=400&maxheight=250&format=jpg");
                                //versions.Add("_medium", "maxwidth=200&maxheight=200&format=jpg");
                                //versions.Add("_large", "maxwidth=600&maxheight=600&format=jpg");

                                //保存各个版本的缩略图
                                foreach (var key in versions.Keys)
                                {
                                    hpf.InputStream.Seek(0, SeekOrigin.Begin);
                                    ImageBuilder.Current.Build(new ImageJob(
                                        hpf.InputStream,
                                        pathForSaving + date + key, //不带后缀名的图片名称
                                        new Instructions(versions[key]),
                                        false,//是否保留原图
                                        true));//是否增加后缀
                                }

                                results.Add(new UploadFileResult()
                                {
                                    FileName = date + "_small" + ".jpg",
                                    FilePath = Url.Content(String.Format("~/AjaxUpload/{0}", date + "_small" + ".jpg")),
                                    IsValid = true,
                                    Length = hpf.ContentLength,
                                    Message = "上传成功",
                                    Type = hpf.ContentType
                                });
                            }
                          
                        }
                    }
                }
            }

            return Json(new
            {
                filename = results[0].FileName,
                filepath=results[0].FilePath,
                isvalid=results[0].IsValid,
                length=results[0].Length,
                message=results[0].Message,
                type=results[0].Type
            });
        }

        //根据文件名删除文件
        [HttpPost]
        public ActionResult DeleteFileByName(string smallname)
        {
            string pathForSaving = Server.MapPath("~/AjaxUpload");
            System.IO.File.Delete(Path.Combine(pathForSaving, smallname));
            return Json(new
            {
                msg = true
            });
        }

        //根据相对路径在项目根路径下创建文件夹
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
