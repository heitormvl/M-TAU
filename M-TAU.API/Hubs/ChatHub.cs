using M_TAU.Application.Dtos.Chat;
using M_TAU.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace M_TAU.API.Hubs;

[Authorize]
public sealed class ChatHub(IChatSessionService chatSessionService) : Hub
{
    public Task JoinSession(Guid sessionId)
        => Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());

    public Task LeaveSession(Guid sessionId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId.ToString());

    public async Task SendMessage(MessageCreateDto messageDto)
    {
        var saved = await chatSessionService.SendMessageAsync(messageDto);
        await Clients.Group(messageDto.ChatSessionId.ToString())
            .SendAsync("ReceiveMessage", saved);
    }
}
