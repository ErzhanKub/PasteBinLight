using Application.Contracts;

namespace Tests.TheoryDataObjects;

public class RecordDtoData : TheoryData<RecordDto>
{
    public RecordDtoData()
    {
        var record = new RecordDto
        {
            Id = Guid.NewGuid(),
            Text = "Sample text for the record",
            Title = "Record Title",
            DeadLine = DateTime.Now.AddDays(7),
            IsPrivate = false,
        };

        Add(record);
    }
}
