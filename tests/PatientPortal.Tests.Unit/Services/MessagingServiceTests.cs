using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for invalid conversation input (e.g., null subject/message, no recipients selected, missing sender link).
// TODO: Add tests for replying to non-existent conversations and permission checks on participant membership.
// TODO: Add boundary tests for unread counts and large recipient lists.
public class MessagingServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly MessagingService _messagingService;

    public MessagingServiceTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        _messagingService = new MessagingService(_context);
    }

    #region Helper Methods

    private Staff CreateStaff(string firstName, string lastName, string username)
    {
        var staff = new Staff
        {
            FirstName = firstName,
            LastName = lastName,
            StaffUsername = username,
            Password = "hashedpassword",
            Role = "Doctor",
            IsAdmin = false,
            MessagingLink = new MessagingLink()
        };
        _context.Staff.Add(staff);
        _context.SaveChanges();
        return staff;
    }

    private Patient CreatePatient(string firstName, string lastName)
    {
        var patient = new Patient
        {
            FirstName = firstName,
            LastName = lastName,
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = "1234",
            MessagingLink = new MessagingLink()
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();
        return patient;
    }

    private static int GetMessagingLinkId(object entity) => entity switch
    {
        Staff s when s.MessagingLink is not null => s.MessagingLink.MessagingLinkId,
        Patient p when p.MessagingLink is not null => p.MessagingLink.MessagingLinkId,
        _ => throw new ArgumentException("Entity must be Staff or Patient with a MessagingLink.", nameof(entity))
    };

    #endregion

    #region CreateConversation Tests

    [Fact]
    public void CreateConversation_WithStaffRecipients_CreatesConversationWithParticipants()
    {
        // Arrange
        var sender = CreateStaff("John", "Doctor", "jdoctor");
        var recipient = CreateStaff("Jane", "Nurse", "jnurse");

        var newConversationForm = new NewConversationFormView
        {
            Subject = "Test Subject",
            MessageText = "Hello, this is a test message.",
            Recipients = new List<Recipient>
            {
                new Recipient { LinkId = GetMessagingLinkId(recipient), Selected = true }
            }
        };

        // Act
        _messagingService.CreateConversation(GetMessagingLinkId(sender), newConversationForm);

        // Assert
        var conversation = _context.Conversations
            .Include(c => c.ConversationParticipants)
            .Include(c => c.Messages)
            .FirstOrDefault();

        Assert.NotNull(conversation);
        Assert.Equal("Test Subject", conversation.Subject);
        Assert.False(conversation.WithPatient);
        Assert.Equal(2, conversation.ConversationParticipants.Count); // Sender + 1 recipient
        Assert.Single(conversation.Messages);
        Assert.Equal("Hello, this is a test message.", conversation.Messages.First().MessageText);
    }

    [Fact]
    public void CreateConversation_WithPatientRecipient_IncludesPatientInParticipants()
    {
        // Arrange
        var sender = CreateStaff("John", "Doctor", "jdoctor");
        var patient = CreatePatient("Bob", "Patient");

        var newConversationForm = new NewConversationFormView
        {
            Subject = "Patient Inquiry",
            MessageText = "How are you feeling today?",
            Recipients = new List<Recipient>(),
            PatientRecipient = new Recipient { LinkId = GetMessagingLinkId(patient), Selected = true }
        };

        // Act
        _messagingService.CreateConversation(GetMessagingLinkId(sender), newConversationForm);
        // Assert
        var conversation = _context.Conversations
            .Include(c => c.ConversationParticipants)
            .FirstOrDefault();

        Assert.NotNull(conversation);
        Assert.True(conversation.WithPatient);
        Assert.Equal(2, conversation.ConversationParticipants.Count); // Sender + Patient
    }

    [Fact]
    public void CreateConversation_SetsUnreadForRecipients()
    {
        // Arrange
        var sender = CreateStaff("John", "Doctor", "jdoctor");
        var recipient = CreateStaff("Jane", "Nurse", "jnurse");

        var newConversationForm = new NewConversationFormView
        {
            Subject = "Urgent",
            MessageText = "Please check this.",
            Recipients = new List<Recipient>
            {
                new Recipient { LinkId = GetMessagingLinkId(recipient), Selected = true }
            }
        };

        // Act
        _messagingService.CreateConversation(GetMessagingLinkId(sender), newConversationForm);

        // Assert
        var message = _context.Messages
            .Include(m => m.UnreadBy)
            .FirstOrDefault();

        Assert.NotNull(message);
        Assert.Single(message.UnreadBy); // Only recipient should have unread flag
        Assert.Equal(GetMessagingLinkId(recipient), message.UnreadBy.First().MessagingLinkId);
    }

    [Fact]
    public void CreateConversation_WithMultipleRecipients_AddsAllParticipants()
    {
        // Arrange
        var sender = CreateStaff("John", "Doctor", "jdoctor");
        var recipient1 = CreateStaff("Jane", "Nurse", "jnurse");
        var recipient2 = CreateStaff("Bob", "Admin", "badmin");

        var newConversationForm = new NewConversationFormView
        {
            Subject = "Team Meeting",
            MessageText = "Let's discuss the patient.",
            Recipients = new List<Recipient>
            {
                new Recipient { LinkId = GetMessagingLinkId(recipient1), Selected = true },
                new Recipient { LinkId = GetMessagingLinkId(recipient2), Selected = true }
            }
        };

        // Act
        _messagingService.CreateConversation(GetMessagingLinkId(sender), newConversationForm);

        // Assert
        var conversation = _context.Conversations
            .Include(c => c.ConversationParticipants)
            .FirstOrDefault();

        Assert.NotNull(conversation);
        Assert.Equal(3, conversation.ConversationParticipants.Count); // Sender + 2 recipients
    }

    [Fact]
    public void CreateConversation_WithUnselectedRecipients_OnlyAddsSelectedRecipients()
    {
        // Arrange
        var sender = CreateStaff("John", "Doctor", "jdoctor");
        var selectedRecipient = CreateStaff("Jane", "Nurse", "jnurse");
        var unselectedRecipient = CreateStaff("Bob", "Admin", "badmin");

        var newConversationForm = new NewConversationFormView
        {
            Subject = "Private Message",
            MessageText = "This is private.",
            Recipients = new List<Recipient>
            {
                new Recipient { LinkId = GetMessagingLinkId(selectedRecipient), Selected = true },
                new Recipient { LinkId = GetMessagingLinkId(unselectedRecipient), Selected = false }
            }
        };

        // Act
        _messagingService.CreateConversation(GetMessagingLinkId(sender), newConversationForm);

        // Assert
        var conversation = _context.Conversations
            .Include(c => c.ConversationParticipants)
            .FirstOrDefault();

        Assert.NotNull(conversation);
        Assert.Equal(2, conversation.ConversationParticipants.Count); // Sender + 1 selected recipient
    }

    #endregion

    #region CreateReply Tests

    [Fact]
    public void CreateReply_AddsMessageToConversation()
    {
        // Arrange
        var sender = CreateStaff("John", "Doctor", "jdoctor");
        var recipient = CreateStaff("Jane", "Nurse", "jnurse");

        var conversation = new Conversation
        {
            Subject = "Test",
            WithPatient = false,
            ConversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant { MessagingLinkId = GetMessagingLinkId(sender) },
                new ConversationParticipant { MessagingLinkId = GetMessagingLinkId(recipient) }
            },
            Messages = new List<Message>
            {
                new Message { MessageText = "Original message", MessagingLinkId = GetMessagingLinkId(sender) }
            }
        };
        _context.Conversations.Add(conversation);
        _context.SaveChanges();

        var replyView = new ReplyView { MessageText = "This is a reply." };

        // Act
        _messagingService.CreateReply(GetMessagingLinkId(recipient), conversation.ConversationId, replyView);

        // Assert
        var updatedConversation = _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefault(c => c.ConversationId == conversation.ConversationId);

        Assert.NotNull(updatedConversation);
        Assert.Equal(2, updatedConversation.Messages.Count);
        Assert.Contains(updatedConversation.Messages, m => m.MessageText == "This is a reply.");
    }

    [Fact]
    public void CreateReply_SetsUnreadForOtherParticipants()
    {
        // Arrange
        var sender = CreateStaff("John", "Doctor", "jdoctor");
        var recipient = CreateStaff("Jane", "Nurse", "jnurse");

        var conversation = new Conversation
        {
            Subject = "Test",
            WithPatient = false,
            ConversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant { MessagingLinkId = GetMessagingLinkId(sender) },
                new ConversationParticipant { MessagingLinkId = GetMessagingLinkId(recipient) }
            },
            Messages = new List<Message>()
        };
        _context.Conversations.Add(conversation);
        _context.SaveChanges();

        var replyView = new ReplyView { MessageText = "Reply message" };

        // Act
        _messagingService.CreateReply(GetMessagingLinkId(sender), conversation.ConversationId, replyView);

        // Assert
        var newMessage = _context.Messages
            .Include(m => m.UnreadBy)
            .FirstOrDefault(m => m.MessageText == "Reply message");

        Assert.NotNull(newMessage);
        Assert.Single(newMessage.UnreadBy);
        Assert.Equal(GetMessagingLinkId(recipient), newMessage.UnreadBy.First().MessagingLinkId);
    }

    #endregion

    #region MarkRead Tests

    [Fact]
    public void MarkRead_WithUnreadMessage_RemovesUnreadFlag()
    {
        // Arrange
        var staff = CreateStaff("John", "Doctor", "jdoctor");

        var message = new Message
        {
            MessageText = "Test",
            MessagingLinkId = 999, // Different sender
            ConversationId = 1,
            UnreadBy = new List<Unread>
            {
                new Unread { MessagingLinkId = GetMessagingLinkId(staff), WithPatient = false }
            }
        };
        _context.Messages.Add(message);
        _context.SaveChanges();

        // Act
        var result = _messagingService.MarkRead(GetMessagingLinkId(staff), message.MessageId);

        // Assert
        Assert.True(result);
        var unreadFlags = _context.UnreadMessages
            .Where(u => u.MessageId == message.MessageId && u.MessagingLinkId == GetMessagingLinkId(staff))
            .ToList();
        Assert.Empty(unreadFlags);
    }

    [Fact]
    public void MarkRead_WithAlreadyReadMessage_ReturnsFalse()
    {
        // Arrange
        var staff = CreateStaff("John", "Doctor", "jdoctor");

        var message = new Message
        {
            MessageText = "Test",
            MessagingLinkId = 999,
            ConversationId = 1,
            UnreadBy = new List<Unread>() // No unread flags
        };
        _context.Messages.Add(message);
        _context.SaveChanges();

        // Act
        var result = _messagingService.MarkRead(GetMessagingLinkId(staff), message.MessageId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void MarkRead_WithNonExistentMessage_ReturnsFalse()
    {
        // Arrange
        var staff = CreateStaff("John", "Doctor", "jdoctor");

        // Act
        var result = _messagingService.MarkRead(GetMessagingLinkId(staff), 99999);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetMessagingLink Tests

    [Fact]
    public void GetMessagingLink_WithExistingLink_ReturnsLink()
    {
        // Arrange
        var staff = CreateStaff("John", "Doctor", "jdoctor");

        // Act
        var result = _messagingService.GetMessagingLink(GetMessagingLinkId(staff));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(GetMessagingLinkId(staff), result.MessagingLinkId);
    }

    [Fact]
    public void GetMessagingLink_WithNonExistingLink_ReturnsNull()
    {
        // Act
        var result = _messagingService.GetMessagingLink(99999);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetUnreadCount Tests

    [Fact]
    public void GetUnreadTotalCount_WithUnreadMessages_ReturnsCorrectCount()
    {
        // Arrange
        var staff = CreateStaff("John", "Doctor", "jdoctor");
        var messagingLinkId = GetMessagingLinkId(staff);
        var link = _context.MessagingLinks.Find(messagingLinkId);

        _context.UnreadMessages.AddRange(
            new Unread { MessagingLinkId = messagingLinkId, MessageId = 1, WithPatient = false },
            new Unread { MessagingLinkId = messagingLinkId, MessageId = 2, WithPatient = true },
            new Unread { MessagingLinkId = messagingLinkId, MessageId = 3, WithPatient = false }
        );
        _context.SaveChanges();

        // Act
        var result = _messagingService.GetUnreadTotalCount(link);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public void GetUnreadPatientCount_WithPatientMessages_ReturnsOnlyPatientCount()
    {
        // Arrange
        var staff = CreateStaff("John", "Doctor", "jdoctor");
        var messagingLinkId = GetMessagingLinkId(staff);
        var link = _context.MessagingLinks.Find(messagingLinkId);

        _context.UnreadMessages.AddRange(
            new Unread { MessagingLinkId = messagingLinkId, MessageId = 1, WithPatient = false },
            new Unread { MessagingLinkId = messagingLinkId, MessageId = 2, WithPatient = true },
            new Unread { MessagingLinkId = messagingLinkId, MessageId = 3, WithPatient = true }
        );
        _context.SaveChanges();

        // Act
        var result = _messagingService.GetUnreadPatientCount(link);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public void GetUnreadTotalCount_WithNoUnreadMessages_ReturnsZero()
    {
        // Arrange
        var staff = CreateStaff("John", "Doctor", "jdoctor");
        var messagingLinkId = GetMessagingLinkId(staff);
        var link = _context.MessagingLinks.Find(messagingLinkId);

        // Act
        var result = _messagingService.GetUnreadTotalCount(link);

        // Assert
        Assert.Equal(0, result);
    }

    #endregion

    #region GetRecipients Tests

    [Fact]
    public void GetPatientRecipient_WithExistingPatient_ReturnsRecipient()
    {
        // Arrange
        var patient = CreatePatient("Bob", "Patient");
        var messagingLinkId = GetMessagingLinkId(patient);

        // Act
        var result = _messagingService.GetPatientRecipient(messagingLinkId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(messagingLinkId, result.LinkId);
        Assert.Equal("Bob Patient", result.Name);
        Assert.Equal("Patient", result.Role);
        Assert.True(result.Selected);
    }

    [Fact]
    public void GetPatientRecipient_WithNonExistingPatient_ReturnsNull()
    {
        // Act
        var result = _messagingService.GetPatientRecipient(99999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetPatientRecipient_WithNullLinkId_ReturnsNull()
    {
        // Act
        var result = _messagingService.GetPatientRecipient(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAllOtherStaffAsRecipients_ExcludesCurrentUser()
    {
        // Arrange
        var currentUser = CreateStaff("John", "Doctor", "jdoctor");
        var otherStaff1 = CreateStaff("Jane", "Nurse", "jnurse");
        var otherStaff2 = CreateStaff("Bob", "Admin", "badmin");

        // Act
        var result = _messagingService.GetAllOtherStaffAsRecipients(GetMessagingLinkId(currentUser), null);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, r => r.LinkId == GetMessagingLinkId(currentUser));
    }

    [Fact]
    public void GetAllOtherStaffAsRecipients_WithToLinkId_PreselectsRecipient()
    {
        // Arrange
        var currentUser = CreateStaff("John", "Doctor", "jdoctor");
        var targetStaff = CreateStaff("Jane", "Nurse", "jnurse");
        var otherStaff = CreateStaff("Bob", "Admin", "badmin");
        var currentUserLinkId = GetMessagingLinkId(currentUser);
        var targetStaffLinkId = GetMessagingLinkId(targetStaff);
        var otherStaffLinkId = GetMessagingLinkId(otherStaff);

        // Act
        var result = _messagingService.GetAllOtherStaffAsRecipients(
            currentUserLinkId, 
            targetStaffLinkId);

        // Assert
        var preselected = result.FirstOrDefault(r => r.LinkId == targetStaffLinkId);
        Assert.NotNull(preselected);
        Assert.True(preselected.Selected);

        var notPreselected = result.FirstOrDefault(r => r.LinkId == otherStaffLinkId);
        Assert.NotNull(notPreselected);
        Assert.False(notPreselected.Selected);
    }

    #endregion

    #region GetAllConversationsForInbox Tests

    [Fact]
    public void GetAllConversationsForInbox_ReturnsOnlyUserConversations()
    {
        // Arrange
        var user = CreateStaff("John", "Doctor", "jdoctor");
        var otherUser = CreateStaff("Jane", "Nurse", "jnurse");
        var userLinkId = GetMessagingLinkId(user);
        var otherLinkId = GetMessagingLinkId(otherUser);

        // Conversation user is part of
        var userConversation = new Conversation
        {
            Subject = "User's Conversation",
            WithPatient = false,
            ConversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant { MessagingLinkId = userLinkId }
            },
            Messages = new List<Message>()
        };

        // Conversation user is NOT part of
        var otherConversation = new Conversation
        {
            Subject = "Other's Conversation",
            WithPatient = false,
            ConversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant { MessagingLinkId = otherLinkId }
            },
            Messages = new List<Message>()
        };

        _context.Conversations.AddRange(userConversation, otherConversation);
        _context.SaveChanges();

        var filters = new ConversationSearch { IsPatientInbox = false, OnlyUnread = false };

        // Act
        var result = _messagingService.GetAllConversationsForInbox(userLinkId, filters).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("User's Conversation", result.First().Subject);
    }

    [Fact]
    public void GetAllConversationsForInbox_WithPatientFilter_ReturnsOnlyPatientConversations()
    {
        // Arrange
        var user = CreateStaff("John", "Doctor", "jdoctor");
        var userLinkId = GetMessagingLinkId(user);

        var patientConversation = new Conversation
        {
            Subject = "Patient Conversation",
            WithPatient = true,
            ConversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant { MessagingLinkId = userLinkId }
            },
            Messages = new List<Message>()
        };

        var staffConversation = new Conversation
        {
            Subject = "Staff Conversation",
            WithPatient = false,
            ConversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant { MessagingLinkId = userLinkId }
            },
            Messages = new List<Message>()
        };

        _context.Conversations.AddRange(patientConversation, staffConversation);
        _context.SaveChanges();

        var filters = new ConversationSearch { IsPatientInbox = true, OnlyUnread = false };

        // Act
        var result = _messagingService.GetAllConversationsForInbox(userLinkId, filters).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Patient Conversation", result.First().Subject);
    }

    [Fact]
    public void GetAllConversationsForInbox_WithUnreadFilter_ReturnsOnlyUnreadConversations()
    {
        // Arrange
        var user = CreateStaff("John", "Doctor", "jdoctor");
        var userLinkId = GetMessagingLinkId(user);

        var unreadConversation = new Conversation
        {
            Subject = "Unread Conversation",
            WithPatient = false,
            ConversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant { MessagingLinkId = userLinkId }
            },
            Messages = new List<Message>
            {
                new Message
                {
                    MessageText = "Test",
                    MessagingLinkId = 999,
                    UnreadBy = new List<Unread>
                    {
                        new Unread { MessagingLinkId = userLinkId, WithPatient = false }
                    }
                }
            }
        };

        var readConversation = new Conversation
        {
            Subject = "Read Conversation",
            WithPatient = false,
            ConversationParticipants = new List<ConversationParticipant>
            {
                new ConversationParticipant { MessagingLinkId = userLinkId }
            },
            Messages = new List<Message>
            {
                new Message { MessageText = "Test", MessagingLinkId = 999, UnreadBy = new List<Unread>() }
            }
        };

        _context.Conversations.AddRange(unreadConversation, readConversation);
        _context.SaveChanges();

        var filters = new ConversationSearch { IsPatientInbox = false, OnlyUnread = true };

        // Act
        var result = _messagingService.GetAllConversationsForInbox(userLinkId, filters).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Unread Conversation", result.First().Subject);
    }

    #endregion

    public void Dispose()
    {
        _messagingService?.Dispose();
        _context?.Dispose();
    }
}
