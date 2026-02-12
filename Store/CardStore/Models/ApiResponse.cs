namespace CardStore.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data, string message = "Operation successful")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResult(string message, object? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse SuccessResult(string message = "Operation successful")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static new ApiResponse ErrorResult(string message, object? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

public class PagedApiResponse<T> : ApiResponse<IEnumerable<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;

    public static PagedApiResponse<T> SuccessResult(
        IEnumerable<T> data, 
        int pageNumber, 
        int pageSize, 
        int totalRecords, 
        string message = "Operation successful")
    {
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        
        return new PagedApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        };
    }
}