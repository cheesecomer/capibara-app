﻿using System;

using Capibara.Models;

namespace Capibara.Net
{
    /// <summary>
    /// Request factory.
    /// </summary>
    public interface IRequestFactory
    {
        RequestBase<Block> BlocksCreateRequest(User target);

        RequestBase BlocksDestroyRequest(int blockId);

        RequestBase<Blocks.IndexResponse> BlocksIndexRequest();

        RequestBase<DirectMessages.IndexResponse> DirectMessagesIndexRequest();

        RequestBase<DirectMessages.ShowResponse> DirectMessagesShowRequest(User user, int lastId = 0);

        RequestBase<Informations.IndexResponse> InformationsIndexRequest();

        RequestBase ReportsCreateRequest(User target, ReportReason reason, string message = null);

        RequestBase<Rooms.IndexResponse> RoomsIndexRequest();

        RequestBase<Room> RoomsShowRequest(Room target);

        RequestBase<Sessions.CreateResponse> SessionsRefreshRequest();

        RequestBase<Sessions.CreateResponse> SessionsCreateRequest(string email, string password);

        RequestBase<Sessions.CreateResponse> UsersCreateRequest(string nickname);

        RequestBase UsersDestroyRequest();

        RequestBase<User> UsersShowRequest(User target);

        RequestBase<User> UsersUpdateRequest(User target);

        RequestBase<User> UsersUpdateRequest(bool isAccepted);

        RequestBase InquiriesCreateRequest(string email, string content);

        RequestBase<Follow> FollowsCreateRequest(User target);

        RequestBase FollowsDestroyRequest(int followId);

        RequestBase<Follows.IndexResponse> FollowsIndexRequest();
    }

    public class RequestFactory : IRequestFactory
    {
        RequestBase<Block> IRequestFactory.BlocksCreateRequest(User target)
            => new Blocks.CreateRequest(target);

        RequestBase IRequestFactory.BlocksDestroyRequest(int blockId)
            => new Blocks.DestroyRequest(blockId);

        RequestBase<Blocks.IndexResponse> IRequestFactory.BlocksIndexRequest()
            => new Blocks.IndexRequest();

        RequestBase<DirectMessages.IndexResponse> IRequestFactory.DirectMessagesIndexRequest()
            => new DirectMessages.IndexRequest();

        RequestBase<DirectMessages.ShowResponse> IRequestFactory.DirectMessagesShowRequest(User user, int lastId)
            => new DirectMessages.ShowRequest(user, lastId);

        RequestBase<Informations.IndexResponse> IRequestFactory.InformationsIndexRequest()
            => new Informations.IndexRequest();

        RequestBase IRequestFactory.ReportsCreateRequest(User target, ReportReason reason, string message)
            => new Reports.CreateRequest(target, reason, message);

        RequestBase<Rooms.IndexResponse> IRequestFactory.RoomsIndexRequest()
            => new Rooms.IndexRequest();

        RequestBase<Room> IRequestFactory.RoomsShowRequest(Room target)
            => new Rooms.ShowRequest(target);

        RequestBase<Sessions.CreateResponse> IRequestFactory.SessionsRefreshRequest()
            => new Sessions.ShowRequest();

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

        RequestBase<User> IRequestFactory.UsersUpdateRequest(bool isAccepted)
            => new Users.UpdateRequest(isAccepted);

        RequestBase IRequestFactory.InquiriesCreateRequest(string email, string content)
            => new Inquiries.CreateRequest(email, content);
        
        RequestBase<Follow> IRequestFactory.FollowsCreateRequest(User target)
            => new Follows.CreateRequest(target);

        RequestBase IRequestFactory.FollowsDestroyRequest(int followId)
            => new Follows.DestroyRequest(followId);

        RequestBase<Follows.IndexResponse> IRequestFactory.FollowsIndexRequest()
            => new Follows.IndexRequest();
    }
}
