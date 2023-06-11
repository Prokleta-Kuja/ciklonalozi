namespace ciklonalozi.Models;

public class DateRequestModel
{
    public int Effort { get; set; }
}

public class DateResponseModel
{
    public required string Date { get; set; }
    public required string Text { get; set; }
}