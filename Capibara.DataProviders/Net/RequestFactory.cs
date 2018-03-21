﻿using System;

using Capibara.Models;

namespace Capibara.Net
{
    /// <summary>
    /// Request factory.
    /// </summary>
    public interface IRequestFactory
    {
        RequestBase<Blocks.IndexResponse> BlocksCreateRequest(User target);

        RequestBase BlocksDestroyRequest(Block target);

        RequestBase<Blocks.IndexResponse> BlocksIndexRequest();

        RequestBase<Informations.IndexResponse> InformationsIndexRequest();

        RequestBase ReportsCreateRequest(User target, ReportReason reason, string message = null);

        RequestBase<Rooms.IndexResponse> RoomsIndexRequest();

        RequestBase<Room> RoomsShowRequest(Room target);

        RequestBase<Sessions.CreateResponse> SessionsCreateRequest(string email, string password);

        RequestBase<Sessions.CreateResponse> UsersCreateRequest(string nickname);

        RequestBase UsersDestroyRequest();

        RequestBase<User> UsersShowRequest(User target);

        RequestBase<User> UsersUpdateRequest(User target);
    }

    public class RequestFactory : IRequestFactory
    {
        RequestBase<Blocks.IndexResponse> IRequestFactory.BlocksCreateRequest(User target)
              => new Blocks.CreateRequest(target);

        RequestBase IRequestFactory.BlocksDestroyRequest(Block target)
              => new Blocks.DestroyRequest(target);

        RequestBase<Blocks.IndexResponse> IRequestFactory.BlocksIndexRequest()
              => new Blocks.IndexRequest();

        RequestBase<Informations.IndexResponse> IRequestFactory.InformationsIndexRequest()
                    => new Informations.IndexRequest();

        RequestBase IRequestFactory.ReportsCreateRequest(User target, ReportReason reason, string message)
               => new Reports.CreateRequest(target, reason, message);

        RequestBase<Rooms.IndexResponse> IRequestFactory.RoomsIndexRequest()
             => new Rooms.IndexRequest();

        RequestBase<Room> IRequestFactory.RoomsShowRequest(Room target)
             => new Rooms.ShowRequest(target);

        RequestBase<Sessions.CreateResponse> IRequestFactory.SessionsCreateRequest(string email, string password)
                => new Sessions.CreateRequest(email, password);

        RequestBase<Sessions.CreateResponse> IRequestFactory.UsersCreateRequest(string nickname)
             => new Users.CreateRequest(nickname);

        RequestBase IRequestFactory.UsersDestroyRequest()
             => new Users.DestroyRequest();

        RequestBase<User> IRequestFactory.UsersShowRequest(User target)
             => new Users.ShowRequest(target);

        RequestBase<User> IRequestFactory.UsersUpdateRequest(User target)
             => new Users.UpdateRequest(target);
    }
}