﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Db;

namespace SiteServer.API.Controllers.Pages.Settings.Config
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/configHomeMenu")]
    public class PagesConfigHomeMenuController : ApiController
    {
        private const string Route = "";
        private const string RouteReset = "actions/reset";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = UserMenuManager.GetAllUserMenuInfoList(),
                    Groups = UserGroupManager.GetUserGroupInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                DataProvider.UserMenuDao.Delete(id);

                return Ok(new
                {
                    Value = UserMenuManager.GetAllUserMenuInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit([FromBody] UserMenuInfo menuInfo)
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                if (menuInfo.Id == 0)
                {
                    DataProvider.UserMenuDao.Insert(menuInfo);

                    await request.AddAdminLogAsync("新增用户菜单", $"用户菜单:{menuInfo.Text}");
                }
                else if (menuInfo.Id > 0)
                {
                    DataProvider.UserMenuDao.Update(menuInfo);

                    await request.AddAdminLogAsync("修改用户菜单", $"用户菜单:{menuInfo.Text}");
                }

                return Ok(new
                {
                    Value = UserMenuManager.GetAllUserMenuInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteReset)]
        public async Task<IHttpActionResult> Reset()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                foreach (var userMenuInfo in UserMenuManager.GetAllUserMenuInfoList())
                {
                    DataProvider.UserMenuDao.Delete(userMenuInfo.Id);
                }

                await request.AddAdminLogAsync("重置用户菜单");

                return Ok(new
                {
                    Value = UserMenuManager.GetAllUserMenuInfoList()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
