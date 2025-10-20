using NSubstitute;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for pagination boundaries and ordering (OrderByDescending DateLastMessage).
// TODO: Add tests for UnknownRecipients (linked to neither Staff nor Patient) once that flow is finalised.
public class MessagingViewServiceTests
{
    private readonly IMessagingService _mockMessagingService;
    private readonly MessagingViewService _messagingViewService;

    public MessagingViewServiceTests()
    {
        _mockMessagingService = Substitute.For<IMessagingService>();
        _messagingViewService = new MessagingViewService(_mockMessagingService);
    }

    #region Helper Methods

    private static Staff CreateStaff(int staffId, string firstName, string lastName, string role = "Doctor")
    {
        return new Staff
        {
            StaffId = staffId,
            FirstName = firstName,
            LastName = lastName,
            StaffUsername = $"{firstName.ToLower()}{staffId}",
            Password = "hashed",
            Role = role,
            IsAdmin = false,
        };
    }

    private static Patient CreatePatient(int patientId, string firstName, string lastName)
    {
        return new Patient
        {
            PatientId = patientId,
            FirstName = firstName,
            LastName = lastName,
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = "1234",
        };
    }

    private static ConversationParticipant StaffParticipant(int linkId, Staff staff)
    {
        return new ConversationParticipant
        {
            MessagingLinkId = linkId,
            MessagingLink = new MessagingLink
            {
                MessagingLinkId = linkId,
                StaffId = staff.StaffId,
                Staff = staff,
            }
        };
    }

    private static ConversationParticipant PatientParticipant(int linkId, Patient patient)
    {
        return new ConversationParticipant
        {
            MessagingLinkId = linkId,
            MessagingLink = new MessagingLink
            {
                MessagingLinkId = linkId,
                PatientId = patient.PatientId,
                Patient = patient,
            }
        };
    }

    private void SetupInboxMock(int viewerLinkId, IEnumerable<Conversation> conversations)
    {
        _mockMessagingService
            .GetMessagingLink(viewerLinkId)
            .Returns(new MessagingLink { MessagingLinkId = viewerLinkId });

        _mockMessagingService
            .GetUnreadTotalCount(Arg.Any<MessagingLink>())
            .Returns(0);

        _mockMessagingService
            .GetUnreadPatientCount(Arg.Any<MessagingLink>())
            .Returns(0);

        _mockMessagingService
            .GetAllConversationsForInbox(viewerLinkId, Arg.Any<ConversationSearch>())
            .Returns(conversations.AsQueryable());
    }

    private static Paginator DefaultPaginator() => new Paginator { CurrentPage = 1, ResultsPerPage = 10 };
    private static ConversationSearch StaffInboxFilters() => new ConversationSearch { IsPatientInbox = false };

    #endregion

    #region ReturnInboxView — null guard

    [Fact]
    public void ReturnInboxView_WhenMessagingLinkNotFound_ReturnsNull()
    {
        // Arrange
        _mockMessagingService.GetMessagingLink(Arg.Any<int>()).Returns((MessagingLink?)null);

        // Act
        var result = _messagingViewService.ReturnInboxView(999, StaffInboxFilters(), DefaultPaginator());

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region ReturnInboxView — participant projection

    [Fact]
    public void ReturnInboxView_StaffOnlyConversation_AllParticipantsProjectedToStaffRecipients()
    {
        // Arrange
        int viewerLinkId = 1;
        var viewer = CreateStaff(10, "John", "Viewer");
        var recipient = CreateStaff(20, "Jane", "Nurse");

        var conversation = new Conversation
        {
            Subject = "Staff Chat",
            WithPatient = false,
            ConversationParticipants =
            [
                StaffParticipant(viewerLinkId, viewer),
                StaffParticipant(2, recipient),
            ],
            Messages = [],
        };

        SetupInboxMock(viewerLinkId, [conversation]);

        // Act
        var result = _messagingViewService.ReturnInboxView(viewerLinkId, StaffInboxFilters(), DefaultPaginator());

        // Assert
        Assert.NotNull(result);
        var inboxConversation = Assert.Single(result.Conversations);
        Assert.Equal(2, inboxConversation.StaffRecipients.Count);
        Assert.Null(inboxConversation.PatientRecipient);
    }

    [Fact]
    public void ReturnInboxView_StaffOnlyConversation_PatientRecipientIsNull()
    {
        // Arrange
        int viewerLinkId = 1;
        var viewer = CreateStaff(10, "John", "Viewer");

        var conversation = new Conversation
        {
            Subject = "Solo",
            WithPatient = false,
            ConversationParticipants = [StaffParticipant(viewerLinkId, viewer)],
            Messages = [],
        };

        SetupInboxMock(viewerLinkId, [conversation]);

        // Act
        var result = _messagingViewService.ReturnInboxView(viewerLinkId, StaffInboxFilters(), DefaultPaginator());

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Conversations.Single().PatientRecipient);
    }

    [Fact]
    public void ReturnInboxView_PatientConversation_ProjectsPatientToPatientRecipient()
    {
        // Arrange
        int viewerLinkId = 1;
        var viewer = CreateStaff(10, "John", "Doctor");
        var patient = CreatePatient(30, "Bob", "Patient");

        var conversation = new Conversation
        {
            Subject = "Patient Visit",
            WithPatient = true,
            ConversationParticipants =
            [
                StaffParticipant(viewerLinkId, viewer),
                PatientParticipant(3, patient),
            ],
            Messages = [],
        };

        SetupInboxMock(viewerLinkId, [conversation]);

        // Act
        var result = _messagingViewService.ReturnInboxView(
            viewerLinkId,
            new ConversationSearch { IsPatientInbox = true },
            DefaultPaginator());

        // Assert
        Assert.NotNull(result);
        var inboxConversation = result.Conversations.Single();
        Assert.NotNull(inboxConversation.PatientRecipient);
        Assert.Equal("Bob Patient", inboxConversation.PatientRecipient.Name);
        Assert.Equal("Patient", inboxConversation.PatientRecipient.Role);
        Assert.Equal(3, inboxConversation.PatientRecipient.LinkId);
    }

    [Fact]
    public void ReturnInboxView_PatientConversation_StaffParticipantsStillProjectedToStaffRecipients()
    {
        // Arrange
        int viewerLinkId = 1;
        var viewer = CreateStaff(10, "John", "Doctor");
        var colleague = CreateStaff(20, "Jane", "Nurse");
        var patient = CreatePatient(30, "Bob", "Patient");

        var conversation = new Conversation
        {
            Subject = "Joint Visit",
            WithPatient = true,
            ConversationParticipants =
            [
                StaffParticipant(viewerLinkId, viewer),
                StaffParticipant(2, colleague),
                PatientParticipant(3, patient),
            ],
            Messages = [],
        };

        SetupInboxMock(viewerLinkId, [conversation]);

        // Act
        var result = _messagingViewService.ReturnInboxView(
            viewerLinkId,
            new ConversationSearch { IsPatientInbox = true },
            DefaultPaginator());

        // Assert
        Assert.NotNull(result);
        var inboxConversation = result.Conversations.Single();
        Assert.Equal(2, inboxConversation.StaffRecipients.Count);
        Assert.NotNull(inboxConversation.PatientRecipient);
        Assert.Equal("Bob Patient", inboxConversation.PatientRecipient.Name);
    }

    [Fact]
    public void ReturnInboxView_SetsViewerLinkIdOnEachConversation()
    {
        // Arrange
        int viewerLinkId = 42;
        var viewer = CreateStaff(10, "John", "Doctor");

        var conversations = new List<Conversation>
        {
            new Conversation
            {
                Subject = "First",
                WithPatient = false,
                ConversationParticipants = [StaffParticipant(viewerLinkId, viewer)],
                Messages = [],
            },
            new Conversation
            {
                Subject = "Second",
                WithPatient = false,
                ConversationParticipants = [StaffParticipant(viewerLinkId, viewer)],
                Messages = [],
            },
        };

        SetupInboxMock(viewerLinkId, conversations);

        // Act
        var result = _messagingViewService.ReturnInboxView(viewerLinkId, StaffInboxFilters(), DefaultPaginator());

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Conversations, c => Assert.Equal(viewerLinkId, c.UserLinkId));
    }

    [Fact]
    public void ReturnInboxView_StaffRecipient_NameAndRoleProjectedFromStaffNavigationProperty()
    {
        // Arrange
        int viewerLinkId = 1;
        var viewer = CreateStaff(10, "John", "Doctor");
        var recipient = CreateStaff(20, "Jane", "Nurse", role: "Nurse");

        var conversation = new Conversation
        {
            Subject = "Check",
            WithPatient = false,
            ConversationParticipants =
            [
                StaffParticipant(viewerLinkId, viewer),
                StaffParticipant(2, recipient),
            ],
            Messages = [],
        };

        SetupInboxMock(viewerLinkId, [conversation]);

        // Act
        var result = _messagingViewService.ReturnInboxView(viewerLinkId, StaffInboxFilters(), DefaultPaginator());

        // Assert
        Assert.NotNull(result);
        var recipientEntry = result.Conversations.Single().StaffRecipients
            .Single(r => r.LinkId == 2);
        Assert.Equal("Jane Nurse", recipientEntry.Name);
        Assert.Equal("Nurse", recipientEntry.Role);
    }

    #endregion

}
