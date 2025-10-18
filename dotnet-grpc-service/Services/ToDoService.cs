using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using todogrpcservice.Data;
using todogrpcservice.Models;

namespace todo_grpc_service.Services;

public class ToDoService : ToDoIt.ToDoItBase
{
    private readonly AppDbContext _dbContext;

    public ToDoService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<CreateTodoResponse> CreateToDo(CreateTodoRequest request, ServerCallContext context)
    {
        if (request.Title == string.Empty || request.Description == string.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));
        }

        var toDoItem = new TodoItems
        {
            Title = request.Title,
            Description = request.Description,
            CreatedDate = DateTime.UtcNow,
        };

        await _dbContext.AddAsync(toDoItem);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(new CreateTodoResponse
        {
            Id = toDoItem.Id,
        });

    }

    public override async  Task<ReadTodoResponse> ReadToDo(ReadTodoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (toDoItem != null)
        {
            return await Task.FromResult(new ReadTodoResponse
            {
                Id = toDoItem.Id,
                Title = toDoItem.Title,
                Description = toDoItem.Description,
                ToDoStatus = toDoItem.Status,
                CreatedDate = Convert.ToString(toDoItem.CreatedDate)
            });
        }
        else
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
        }
    }

    public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var toDoItems = await _dbContext.ToDoItems.ToListAsync(); 

        foreach ( var toDoItem in toDoItems )
        {
            response.ToDo.Add(new ReadTodoResponse
            {
                Id = toDoItem.Id,
                Title = toDoItem.Title,
                Description = toDoItem.Description,
                ToDoStatus = toDoItem.Status,
                CreatedDate = toDoItem.CreatedDate.ToString(),
                UpdatedDate = toDoItem.UpdatedDate.ToString()
            });
        }
        return await Task.FromResult(response);

    }

    public override async Task<UpdateTodoResponse> UpdateToDo(UpdateTodoRequest request, ServerCallContext context)
    {
        if(request.Id <= 0 || request.Title == string.Empty || request.Description == string.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));
        }

        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

        if(toDoItem == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with Id {request.Id}"));
        }
        toDoItem.Title = request.Title;
        toDoItem.Description = request.Description;
        toDoItem.Status = request.ToDoStatus;
        toDoItem.UpdatedDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new UpdateTodoResponse
        {
            Id = request.Id
        });
    }

    public override async Task<DeleteTodoResponse> DeleteToDo(DeleteTodoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource Id must be greater than 0"));
        }

        var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (toDoItem == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with Id {request.Id}"));
        }
         _dbContext.Remove(toDoItem);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new DeleteTodoResponse
        {
            Id = request.Id
        });
    }
}

