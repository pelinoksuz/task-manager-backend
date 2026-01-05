public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
}

// neden sadece title?
// çünkü task oluşturulurken id ve iscompleted backend tarafından atanıyor