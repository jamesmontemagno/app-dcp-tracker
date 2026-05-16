using DCPTracker.Models;

namespace DCPTracker.Services;

public interface IDeferralBucketRepository
{
	Task<IReadOnlyList<DeferralBucket>> GetBucketsAsync(CancellationToken cancellationToken = default);

	Task SaveBucketAsync(DeferralBucket bucket, CancellationToken cancellationToken = default);

	Task DeleteBucketAsync(Guid bucketId, CancellationToken cancellationToken = default);
}
