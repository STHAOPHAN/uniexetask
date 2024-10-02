﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class AuthService : IAuthService
    {
        public IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserByRefreshToken(string? refreshToken)
        {
            var rt = await _refreshTokenRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (rt == null)
            {
                return null;
            }
            var user = await _userRepository.GetByIDAsync(rt.UserId);
            if (user == null) 
            {
                return null;
            }
            return user;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _userRepository.AuthenticateAsync(email, password);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async System.Threading.Tasks.Task SaveRefreshToken(int id, string refreshToken)
        {
            await RevokeRefreshToken(id);
            var newRefreshToken = new RefreshToken
            {
                UserId = id,
                Expires = DateTime.UtcNow.AddDays(7),
                Revoked = DateTime.UtcNow.AddDays(7),
                Token = refreshToken,
                Status = true
            };
            await _unitOfWork.RefreshTokens.InsertAsync(newRefreshToken);

            await _unitOfWork.SaveAsync();

        }

        private async System.Threading.Tasks.Task  RevokeRefreshToken(int id)
        {
            var rts = await _refreshTokenRepository.GetRefreshTokensByUserId(id);
            if (rts != null)
            {
                foreach (var token in rts)
                {
                    token.Status = false;
                    token.Revoked = DateTime.Now;
                    _unitOfWork.RefreshTokens.Update(token);
                }
                await _unitOfWork.SaveAsync();
            }

        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                // Log email tìm kiếm
                Console.WriteLine($"Tìm kiếm user với email: {email}");

                var user = await _userRepository.GetUserByEmailAsync(email);

                // Kiểm tra xem user có tồn tại không
                if (user == null)
                {
                    Console.WriteLine("Không tìm thấy user với email này.");
                }

                return user;
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Lỗi khi tìm user: {ex.Message}");
                throw; // Ném lại ngoại lệ để xử lý bên ngoài nếu cần
            }
        }

    }
}
