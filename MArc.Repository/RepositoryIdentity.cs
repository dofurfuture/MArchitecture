﻿using MArc.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using MArc.Models;
using System.Linq.Expressions;
using Microsoft.AspNet.Identity;
using MArc.IRepository;

namespace MArc.Repository
{
    /// <summary>
    /// Identity相关操作
    /// </summary>
    public class RepositoryIdentity : IRepositoryIdentity
    {
        private AppIdentityDbContext context;
        public RepositoryIdentity()
        {
            context = AppIdentityDbContext.Create();
        }
        private AppUserManager UserManager
        {
            get 
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>(); 
                //return AppUserManager.Create();
            }
        }
        private AppRoleManager RoleManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<AppRoleManager>();
                //return AppRoleManager.Create();
            }
        }
        private AppUserManager UserManagerForFind
        {
            get
            {
                return AppUserManager.GetUserManager();
            }
        }
        private AppRoleManager RoleManagerForFind
        {
            get
            {
                return AppRoleManager.GetRoleManager();
            }
        }

        #region AppUser增删查改

        #region AppUser查询
        public IQueryable<AppUser> FindUser()
        {
            return UserManager.Users;
        }
        public IQueryable<AppUser> FindUser(Expression<Func<AppUser, bool>> conditions = null)
        {
            return UserManager.Users.Where(conditions);
        }
        public async Task<AppUser> FindByNameAndPas(string userName, string password)
        {
            return await UserManager.FindAsync(userName, userName);
        }
        public async Task<AppUser> FindUserById(string userId)
        {
            return await UserManager.FindByIdAsync(userId);
        }
        public async Task<AppUser> FindUserByEmail(string email)
        {
            return await UserManager.FindByEmailAsync(email);
        }
        public async Task<AppUser> FindUserByName(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }
        public async Task<AppUser> FindLoginUserByName(string userName)
        {
            return await UserManagerForFind.FindByNameAsync(userName);
        }
        #endregion

        public async Task<bool> CreateUser(AppUser user, string password)
        {
            IdentityResult result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> CreateUser(AppUser user)
        {
            IdentityResult result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateUser(AppUser user)
        {
            IdentityResult result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteUser(AppUser user)
        {
            IdentityResult result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetHashPassword(string password)
        {
            return UserManager.PasswordHasher.HashPassword(password);
        }
        #endregion

        #region AppRole增删查改

        #region AppRole查询
        public IQueryable<AppRole> FindRole()
        {
            return RoleManager.Roles;
        }
        public IQueryable<AppRole> FindRole(Expression<Func<AppRole, bool>> conditions = null)
        {
            return RoleManager.Roles.Where(conditions);
        }
        public async Task<AppRole> FindRoleById(string roleId)
        {
            return await RoleManager.FindByIdAsync(roleId);
        }
        public async Task<AppRole> FindRoleByName(string roleName)
        {
            return await RoleManager.FindByNameAsync(roleName);
        }
        public async Task<bool> RoleExists(string roleName)
        {
            return await RoleManager.RoleExistsAsync(roleName);
        }
        public IQueryable<AppRole> FindRoleGeneral(Expression<Func<AppRole, bool>> conditions = null)
        {
            return RoleManagerForFind.Roles.Where(conditions);
        }
        #endregion

        public async Task<bool> CreateRole(AppRole role)
        {
            IdentityResult result = await RoleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateRole(AppRole role)
        {
            IdentityResult result = await RoleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteRole(AppRole role)
        {
            IdentityResult result = await RoleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public async Task<IQueryable<AppUser>> FindUserInToRole(string roleId)
        {
            AppRole role = await RoleManager.FindByIdAsync(roleId);
            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();
            return UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
        }

        public async Task<IQueryable<AppUser>> FindUserNotInToRole(string roleId)
        {
            AppRole role = await RoleManager.FindByIdAsync(roleId);
            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();
            IQueryable<AppUser> members = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));
            IQueryable<AppUser> nonMembers = UserManager.Users.Except(members);
            return nonMembers;
        }

        public async Task<bool> AddToRole(string userId, string roleName)
        {
            IdentityResult result = await UserManager.AddToRoleAsync(userId, roleName);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromRole(string userId, string roleName)
        {
            IdentityResult result = await UserManager.RemoveFromRoleAsync(userId, roleName);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region token刷新相关
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

            var existingToken = context.RefreshToken.SingleOrDefault(r => r.Subject == token.Subject);

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            context.RefreshToken.Add(token);

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await context.RefreshToken.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                context.RefreshToken.Remove(refreshToken);
                return await context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            context.RefreshToken.Remove(refreshToken);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await context.RefreshToken.FindAsync(refreshTokenId);

            return refreshToken;
        } 
        #endregion



        public void Dispose()
        {
            if (UserManager != null)
            {
                UserManager.Dispose();
            };
            if (RoleManager != null)
            {
                RoleManager.Dispose();
            };
            if (context != null)
            {
                context.Dispose();
            };
        }
    }
}
