using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientPortal.Models;
using PatientPortal.SeedTool.Features.MessageSeeding.DTOs;

namespace PatientPortal.SeedTool.Features.MessageSeeding.Services;

/// <summary>
/// Seeds conversations and messages for existing patients and staff.
/// Targets any MessagingLink with fewer than the conversation threshold.
/// </summary>
public class MessagingSeedService
{
    private readonly PatientPortalContext _context;
    private readonly ILogger<MessagingSeedService> _logger;
    private readonly MessagingDataComposer _messagingDataComposer;

    private const int ConversationThreshold = 2;

    public MessagingSeedService(
        PatientPortalContext context,
        ILogger<MessagingSeedService> logger,
        MessagingDataComposer messagingDataComposer
    )
    {
        _context = context;
        _logger = logger;
        _messagingDataComposer = messagingDataComposer;
    }

    /// <summary>
    /// Seeds conversations for all MessagingLinks with fewer than the conversation threshold.
    /// </summary>
    /// <returns>Number of conversations created</returns>
    public async Task<(int, int)> SeedMessagingAsync()
    {
        int patientConversationsSeeded = 0;
        int staffConversationsSeeded = 0;

        // Queries for MessagingLinks under thresholds
        var patientLinksUnderThresholdQuery = _context
            .MessagingLinks.Where(ml => ml.PatientId != null)
            .Where(ml => ml.ParticipatingConversations.Count < ConversationThreshold);

        var staffLinksUnderThresholdQuery = _context
            .MessagingLinks.Where(ml => ml.StaffId != null)
            .Where(ml =>
                ml.ParticipatingConversations.Count(cp => cp.Conversation.WithPatient == false)
                < ConversationThreshold
            );

        // Projection to DTOs for conversation creation
        List<ConversationDTO> patientsConversationsDTOs = await patientLinksUnderThresholdQuery
            .Select(ml => new ConversationDTO
            {
                PrimaryLinkInfo = new ParticipantDTO
                {
                    MessagingLinkId = ml.MessagingLinkId,
                    CreatedAt = ml.CreatedAt,
                },
                ConversationCount = ml.ParticipatingConversations.Count,
                PotentialCorrespondentInfos = ml.Patient.MedicalTeam
                    .Select(mt => new ParticipantDTO
                    {
                        MessagingLinkId = mt.Staff.MessagingLink.MessagingLinkId,
                        CreatedAt = mt.Staff.MessagingLink.CreatedAt,
                    })
                    .ToList()
            })
            .ToListAsync();

        List<ParticipantDTO> allStaffParticipantInfo = await _context
            .Staff.Select(s => new ParticipantDTO
            {
                MessagingLinkId = s.MessagingLink.MessagingLinkId,
                CreatedAt = s.MessagingLink.CreatedAt,
            })
            .ToListAsync();

        List<ConversationDTO> staffConversationDTOs = await staffLinksUnderThresholdQuery
            .Select(ml => new ConversationDTO
            {
                PrimaryLinkInfo = new ParticipantDTO
                {
                    MessagingLinkId = ml.MessagingLinkId,
                    CreatedAt = ml.CreatedAt,
                },
                ConversationCount = ml.ParticipatingConversations.Count(cp =>
                    cp.Conversation.WithPatient == false
                ),
                PotentialCorrespondentInfos = new List<ParticipantDTO>(),
            })
            .ToListAsync();

        foreach (var staffDTO in staffConversationDTOs)
        {
            staffDTO.PotentialCorrespondentInfos = allStaffParticipantInfo
                .Where(info => info.MessagingLinkId != staffDTO.PrimaryLinkInfo.MessagingLinkId)
                .ToList();
        }

        // Generate conversations

        List<Conversation> patientConversations = [];
        if (patientsConversationsDTOs.Count > 0)
        {
            patientConversations = _messagingDataComposer.CreateConversationsForPatients(
                patientsConversationsDTOs
            );
        }

        List<Conversation> staffConversations = [];
        if (staffConversationDTOs.Count > 0)
        {
            staffConversations = _messagingDataComposer.CreateConversationsForStaffToStaff(
                staffConversationDTOs
            );
        }

        _context.Conversations.AddRange(patientConversations);
        _context.Conversations.AddRange(staffConversations);
        await _context.SaveChangesAsync();

        patientConversationsSeeded = patientConversations.Count;
        staffConversationsSeeded = staffConversations.Count;
        _logger.LogInformation(
            "Successfully seeded {PatientCount} patient and {StaffCount} staff conversations",
            patientConversationsSeeded,
            staffConversationsSeeded
        );

        return (patientConversationsSeeded, staffConversationsSeeded);
    }
}
