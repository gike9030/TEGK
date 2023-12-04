using FlashcardsApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;

public static class UserManagerMock
{
    public static Mock<UserManager<FlashcardsAppUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<FlashcardsAppUser>>();
        var userManager = new Mock<UserManager<FlashcardsAppUser>>(
            store.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        userManager.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                           .ReturnsAsync(new FlashcardsAppUser { });

        return userManager;
    }
}

