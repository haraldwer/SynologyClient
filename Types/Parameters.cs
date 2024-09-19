
namespace Synology.Parameters
{
    public enum ListShareAdditionalParameters
    {
        real_path,
        size,
        owner,
        time,
        perm,
        mount_point_type,
        volume_status
    }

    public enum ListSortMode
    {
        name,
        size,
        user,
        group,
        mtime,
        atime,
        ctime,
        crtime,
        posix,
        type
    }

    public enum ListSortDirection
    {
        asc,
        desc
    }

    public enum ListFileType
    {
        file,
        dir,
        all
    }

    public enum ListAdditionalParameters
    {
        real_path,
        size,
        owner,
        time,
        perm,
        mount_point_type,
        type
    }

    public enum UploadFileExistBehaviorParameter
    {
        throw_error,
        overwrite,
        skip,
    }
}
