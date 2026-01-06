using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rsbc.Dmf.CaseManagement.Service
{
    public class PartnerPortalUserService : PortalPartnerUserManager.PortalPartnerUserManagerBase
    {
        private readonly IPortalPartnerUserManager _userManager;
        private readonly IMapper _mapper;

        public PartnerPortalUserService(IPortalPartnerUserManager userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async override Task<SystemUsersSearchReply> SearchContacts(UsersSearchRequest request, ServerCallContext context)
        {
            try
            {
                var users = (await _userManager.SearchSystemUsers(new SearchPortalPatnerUsersRequest
                {
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    ByUserId = request.UserId.Trim(),
                    ActiveUser = request.ActiveUser,
                    UnauthorizedOnly = request.UnauthorizedOnly,
                    PortalType = request.UserType == UserType.PartnerPortalUserType ? Rsbc.Dmf.CaseManagement.PortalType.PartnerPortal : null

                })).Select(u =>
                {
                    var contact = new Contact
                    {
                        Id = u.Id.ToString(),
                        Active = u.Active,
                        FirstName = u.FirstName ?? string.Empty,
                        SecondGivenName = u.SecondGivenName ?? string.Empty,
                        ThirdGivenName = u.ThirdGivenName ?? string.Empty,
                        LastName = u.LastName ?? string.Empty,
                        AddressLine1 = u.AddressLine1 ?? string.Empty,
                        AddressLine2 = u.AddressLine2 ?? string.Empty,
                        City = u.City ?? string.Empty,
                        Province = u.Province ?? string.Empty,
                        PostCode = u.PostCode ?? string.Empty,
                        Country = u.Country ?? string.Empty,
                        PhoneNumber = u.PhoneNumber ?? string.Empty,
                        CellNumber = u.CellNumber ?? string.Empty,
                        Email = u.Email ?? string.Empty,
                        DFWebuserId = u.DFWebuserId ?? string.Empty,
                        Domain = u.Domain ?? string.Empty,
                        UserName = u.UserName ?? string.Empty,
                        EffectiveDate = u.EffectiveDate != null ? Timestamp.FromDateTimeOffset((DateTimeOffset)u.EffectiveDate) : null,
                        Authorized = u.Authorized,
                    };

                    if (u.ExpiryDate.HasValue)
                    {
                        contact.ExpiryDate = Timestamp.FromDateTimeOffset(u.ExpiryDate.Value);
                    }

                    if (u.EffectiveDate.HasValue)
                    {
                        contact.ExpiryDate = Timestamp.FromDateTimeOffset(u.ExpiryDate.Value);
                    }

                    if (u.UserRoles != null)
                    {
                        var userRoles = u.UserRoles.Where(x => x.Id != null).Select(r => new ContactRoles
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description
                        });
                        contact.UserRoles.AddRange(
                            userRoles
                        );
                    }

                    if (u.AuditDetails != null)
                    {
                        var auditDetails = u.AuditDetails.Select(r => new AuditDetails
                        {
                            EntryId = r.EntryId ?? string.Empty,
                            EntryDate = Timestamp.FromDateTimeOffset(r.EntryDate.Value),
                            Description = r.Description ?? string.Empty
                        });
                        contact.AuditDetails.AddRange(auditDetails);
                    }

                    return contact;
                });

                return new SystemUsersSearchReply { ResultStatus = ResultStatus.Success, User = { users } };
            }
            catch (Exception ex)
            {
                return new SystemUsersSearchReply
                {
                    ResultStatus = ResultStatus.Fail,
                    ErrorDetail = ex.Message
                };
            }
        }
        public async override Task<ResultStatusReply> UpdateContact(Contact contact, ServerCallContext context)
        {
            try
            {
                var portalUser = new PortalUser
                {
                    Id = new Guid(contact.Id),
                    FirstName = contact.FirstName ?? string.Empty,
                    SecondGivenName = contact.SecondGivenName ?? string.Empty,
                    ThirdGivenName = contact.ThirdGivenName ?? string.Empty,
                    LastName = contact.LastName ?? string.Empty,
                    AddressLine1 = contact.AddressLine1 ?? string.Empty,
                    AddressLine2 = contact.AddressLine2 ?? string.Empty,
                    City = contact.City ?? string.Empty,
                    Province = contact.Province ?? string.Empty,
                    PostCode = contact.PostCode ?? string.Empty,
                    Country = contact.Country ?? string.Empty,
                    PhoneNumber = contact.PhoneNumber ?? string.Empty,
                    CellNumber = contact.CellNumber ?? string.Empty,
                    Email = contact.Email ?? string.Empty,
                    DFWebuserId = contact.DFWebuserId ?? string.Empty,
                    Domain = contact.Domain ?? string.Empty,
                    ExpiryDate = contact.ExpiryDate?.ToDateTimeOffset(),
                    EffectiveDate = contact.EffectiveDate?.ToDateTimeOffset(),
                    Authorized = true,
                    ModifiedBy = contact.ModifiedBy
                };
                await _userManager.UpdateContact(portalUser);
                return new ResultStatusReply
                {
                    ResultStatus = ResultStatus.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultStatusReply
                {
                    ResultStatus = ResultStatus.Fail
                };
            }
        }

        public async override Task<ResultStatusReply> UpdateContactRole(UpdateContactRoleRequest request, ServerCallContext context)
        {
            try
            {
                if (request.AddRole)
                {
                    await _userManager.AddContactRole(request.RoleId, request.ContactId, request.ModifiedBy);
                }
                else
                {
                    await _userManager.RemoveContactRole(new Guid(request.RoleId), new Guid(request.ContactId));
                }
                return new ResultStatusReply
                {
                    ResultStatus = ResultStatus.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultStatusReply
                {
                    ResultStatus = ResultStatus.Fail,
                    ErrorDetail = ex.Message
                };
            }
        }

        public async override Task<GetContactRolesStatusReply> GetContactRoles(GetContactRolesRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _userManager.GetContactRoles();

                var roleList = result.Select(r => new ContactRoles
                {
                    Description = r.Description,
                    Name = r.Name,
                    Id = r.Id
                });

                var reply = new GetContactRolesStatusReply
                {
                    ResultStatus = ResultStatus.Success
                };

                reply.Role.AddRange(roleList);

                return reply;
            }
            catch (Exception ex)
            {
                return new GetContactRolesStatusReply
                {
                    ResultStatus = ResultStatus.Fail,
                    ErrorDetail = ex.Message
                };
            }
        }

        public async override Task<GetCurrentLoginUserReply> GetCurrentLoginUser(GetCurrentLoginUserRequest request, ServerCallContext context)
        {
            var userDetails = await _userManager.GetCurrentLoginUser(request.UserId);
            var result = new GetCurrentLoginUserReply();

            result.UserRoles.AddRange(userDetails.UserRoles);
            return result;


        }

    }
}