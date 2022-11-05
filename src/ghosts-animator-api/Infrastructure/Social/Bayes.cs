namespace Ghosts.Animator.Api.Infrastructure.Social;

public class Bayes
{
    public readonly float LikelihoodH1;
    public readonly float PriorH1;
    public readonly float LikelihoodH2;
    public readonly float PriorH2;
    public float PosteriorH1;
    public float PosteriorH2;
    public int Position;

    /// <summary>
    /// straight iterative bayes calculation, where priors become the previous posterior
    /// </summary>
    public Bayes(int position, float likelihood_h_1, float prior_h_1, float likelihood_h_2, float prior_h_2)
    {
        this.Position = position;
        this.LikelihoodH1 = likelihood_h_1;
        this.LikelihoodH2 = likelihood_h_2;
        this.PriorH1 = prior_h_1;
        this.PriorH2 = prior_h_2;
        this.PosteriorH1 = 0;
        this.PosteriorH2 = 0;
        this.CalculatePosterior();
    }

    /// <summary>
    /// Bayes calculation e.g.
    /// Likelihood(H_1)
    /// Prior(H_1)
    /// Likelihood(MG)
    /// Prior(H1)
    /// Likelihood(MB)
    /// Prior(H2)
    /// </summary>
    private void CalculatePosterior()
    {
        if (((this.LikelihoodH1 * this.PriorH1) + (this.LikelihoodH2 * this.PriorH2)) > 0)
        {
            this.PosteriorH1 = (this.LikelihoodH1 * this.PriorH1) /
                                 ((this.LikelihoodH1 * this.PriorH1) + (this.LikelihoodH2 * this.PriorH2));
        }
        else
        {
            this.PosteriorH1 = 0;
        }

        if (((this.LikelihoodH2 * this.PriorH2) + (this.LikelihoodH1 * this.PriorH1)) > 0)
        {
            this.PosteriorH2 = (this.LikelihoodH2 * this.PriorH2) /
                                 ((this.LikelihoodH2 * this.PriorH2) + (this.LikelihoodH1 * this.PriorH1));
        }
        else
        {
            this.PosteriorH2 = 0;
        }

        this.PosteriorH1 = Normalize(this.PosteriorH1);
        this.PosteriorH2 = Normalize(this.PosteriorH2);
    }

    private static float Normalize(float n)
    {
        if (n > 1)
        {
            n = 1;
        }

        if (n < 0)
        {
            n = 0;
        }

        return n;
    }
}