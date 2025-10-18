namespace todogrpcservice.Models;

public class TodoItems
{
    public int Id { get; set; } 
    public string Title { get; set; }   
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set;}
    public string Status { get; set; } = "NEW";

}

